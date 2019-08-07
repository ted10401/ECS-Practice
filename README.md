# Unity ECS 初步認識

## Entities

<p align="center">
<img style="margin:auto;"  src="https://github.com/ted10401/ECS-Practice/blob/master/GithubResources/unity_ecs_entities.png">
</p>
  
  ***
  
Entities 是 ECS 架構中的第一個原則 E  
Entity 本質是針對 Component 的索引  
基本由一個 Index 及 Version 組成  
可以直接將它視為一個超級輕量級的 GameObject 但不包含任何 Component 資料與行為  
實際的 Component 資料會存放在 Chunk 中  
當需要操作 Component 的時候會根據 Index 到 EntityDataManager 中找到 Chunk 的所在位置並進行操作  

***

### Entity 是如何進行分類的？

由於 Entity 本身只包含 ID  
所以分類形成另一個值得探討的問題  
  
EntityManager 會追蹤 World 中的所有 Entities  
並在創建 Entity、添加 Component、刪除 Component 時將 Entity 進行分類  
每一種獨特的 Component 組合都會形成一種 Unity 特有的資料結構 Archetype  
當 Entity 所關聯的資料改變時會隨著 Archetype 被放置到所屬的類別中  
　　
***

### 如何創建 Entity？

#### 透過 GameObject 轉換

目前的 Unity.Entities 測試版本中包含了一個 ConvertToEntity Component  
這個 Component 是傳統的 MonoBehaviour  
能夠在運行時將 GameObject 主動轉為 Entity  
這個方法需要經過 Instantiate -> ConvertToEntity -> Destroy 生成週期  
導致執行時會出現 cpu peak 的問題  
  
#### 透過 EntityManager 生成

EntityManager.CreateEntiy();  
創建空白 Entity  
  
EntityManager.CreateEntity(params ComponentType[] types);  
創建具有複數 Component 特性的 Entity  
  
EntityManager.CreateEntity(EntityArchetype Archetype);  
使用 Archetype 創建具有該 Archetype 特性的 Entity  
  
EntityManager.CreateEntity(EntityArchetype Archetype, NativeArray<Entity> entities);  
使用 Archetype 創建複數 Entities  
  
#### 透過 EntityManager 複製

EntityManager.Instantiate (Entity srcEntity);  
生成具有參考 Entity 特性的 Entity  
  
EntityManager.Instantiate (Entity srcEntity, NativeArray<Entity> outputEntities);  
生成具有參考 Entity 特性的複數 Entities  

***

### 如何刪除 Entity？
  
EntityManager.DestroyEntity (Entity entity);  
EntityManager.DestroyEntity (NativeSlice<Entity> entities);  
EntityManager.DestroyEntity (NativeArray<Entity> entities);  
EntityManager.DestroyEntity (EntityQuery entityQueryFilter);  

## Component

<p align="center">
<img style="margin:auto;"  src="https://github.com/ted10401/ECS-Practice/blob/master/GithubResources/unity_ecs_component.png">
</p>

***

Component 是 ECS 架構中的第二個原則 C  
Component 本質代表了遊戲或專案中的資料  

***

### Component 是如何存放的？

EntityManager 會透過 Archetype 來管理不同的 Component 組合  
將擁有相同 Archetype 的 Entities 存儲在內存區塊 Chunk 中  
在同一個 Chunk 中的所有實體都擁有相同的 Component 組合  
    
在 Entity 添加或刪除 Component  
抑或是修改 SharedComponent 數據時  
EntityManager 會將 Entity 移動至其他 Chunk 中  
若必要則直接產生新 Chunk 供存放

***

### 如何建立 Component?

只要擁有下列這些接口的組件就代表為 ECS 中的 Component  
IComponentData  
ISharedComponentData  
ISystemStateComponentData  
ISharedSystemStateComponentData  
    
通常繼承 IComponentData 或 ISharedComponentData  
兩個接口都是空接口單純為了進行組件標記  
兩者最大的差異是  
  
IComponentData 中的數據是個別獨立的  
每一組 IComponentData 在添加時  
都會在 Chunk 中獨立佔用一份資源  
  
ISharedComponentData 則會將數據共享於複數 Entities 中  
例：Unity.Rendering.RenderMesh  
但過度使用 SharedComponentData 會導致 Chunk 使用率降低  
因為它會根據組件中每種唯一值組合去擴展所需要的 Chunk 數  

***

### 如何添加 Components?

#### 建立 Entity 時添加

EntityManager.CreateEntity (params ComponentType[] types)

#### 動態添加

##### IComponentData

EntityManager.AddChunkComponentData<T> (EntityQuery entityQuery, T componentData) where T : struct, IComponentData;  
EntityManager.AddChunkComponentData<T> (Entity entity);  
EntityManager.AddComponent (NativeArray<Entity> entities, ComponentType componentType);  
EntityManager.AddComponent (EntityQuery entityQuery, ComponentType componentType);  
EntityManager.AddComponent (Entity entity, ComponentType componentType);  
EntityManager.AddComponentData<T> (Entity entity, T componentData)  
EntityManager.AddComponentObject (Entity entity, object componentData);  
EntityManager.AddComponentRaw (Entity entity, int typeIndex);  
EntityManager.AddComponents (Entity entity, ComponentTypes types);  

##### ISharedComponentData

EntityManager.AddSharedComponentData<T> (EntityQuery entityQuery, T componentData)  
EntityManager.AddSharedComponentData<T> (Entity entity, T componentData)  

#### 使用 Archetype 建立

EntityManager.CreateEntity (EntityArchetype Archetype);  
EntityManager.CreateEntity (EntityArchetype Archetype, NativeArray<Entity> entities);  

***

### 如何刪除 Components?

EntityManager.RemoveChunkComponent<T> (Entity entity);  
EntityManager.RemoveChunkComponentData<T> (EntityQuery entityQuery);  
EntityManager.RemoveComponent<T> (Entity entity);  
EntityManager.RemoveComponent (NativeArray<Entity> entities, ComponentType type);  
EntityManager.RemoveComponent (EntityQuery entityQueryFilter, ComponentTypes types);  
EntityManager.RemoveComponent (EntityQuery entityQuery, ComponentType componentType);  
EntityManager.RemoveComponent (Entity entity, ComponentType type);  

***

### 如何修改 Components?

SetChunkComponentData<T> (ArchetypeChunk chunk, T componentValue);  
SetComponentData<T> (Entity entity, T componentData);  
SetSharedComponentData<T> (Entity entity, T componentData);  

***

### 添加、刪除 Components 時需要注意什麼?

上面有提到當 Entity 添加或刪除 Component 時  
由於該 Entity Component 組合改變會連帶改變 Archetype  
EntityManager 會將這些修改過的數據移動到新的內存塊存放  
  
而一般狀況下可以盡情的添加或刪除  
但若在 Job 中添加或刪除 Component 會導致 Job 正在處理的資料無效  
所以不要在 Job 內執行添加或刪除 Component 的動作  
取而代之的  
可以使用 [EntityCommandBuffer] 在 Job 完成後執行添加或修改的需求  

## System

<p align="center">
<img style="margin:auto;"  src="https://github.com/ted10401/ECS-Practice/blob/master/GithubResources/unity_ecs_system.png">
</p>

***

System 是 ECS 架構中的第三個原則 S  
負責針對特定 Component 的邏輯行為  
  
由於 ECS 的分層設計相當明確  
每個層級需要負責的工作都遵循 SOLID 中的單一職責原則（Single Responsibility Principle）  
E: Entity，負責索引  
C: Component，負責資料存儲  
S: System，負責邏輯行為  

***

### 如何建立 System?

#### 繼承 ComponentSystem

只會在主線程執行

#### 繼承 JobComponentSystem

利用 Unity 特有的 JobSystem 來進行多線程處理  
但需要額外使用 JobHandle、IJob、IJobForEach 等進行配合

***

### 如何使用 System?

#### ComponentSystemBase

無論是 ComponentSystem 或 JobComponentSystem 都繼承於 ComponentSystemBase  
提供了幾個虛函數供外部複寫  
  
ComopnentSystemBase.OnCreateManager();  
ComopnentSystemBase.OnDestroyManager();  
ComopnentSystemBase.OnCreate();  
ComopnentSystemBase.OnStartRunning();  
ComopnentSystemBase.OnStopRunning();  
ComopnentSystemBase.OnDestroy();  
ComopnentSystemBase.OnBeforeCreateInternal(World world);  
ComopnentSystemBase.OnBeforeDestroyInternal();  

#### OnUpdate

此外 ComponentSystem 和 JobComponentSystem 都分別提供了 OnUpdate 抽象函數  
ComponentSystem.OnUpdate();  
JobComponentSystem.OnUpdate(JobHandle inputDeps);  
讓開發者能夠針對不同的 System 處理不同的 per frame logic  

***

### 如何取得 Entities 及 Components?

創建 Entity 及賦予 Component 特性後
如何使用這些 Component 特性來處理邏輯行為就是 System 最重要的工作
　　
#### ComponentSystem

能夠使用 Entities 來取得 EntityQueryBuilder
並透過 ForEach 匹配出具有該系統需要的 Component 特性的 Entity

```c#
Entities.ForEach((ref Translation translation) =>  
{  
　　translation.Value.y = math.sin(deltaTime);   
});  
```

#### JobComponentSystem

JobComponentSystem 則較為特殊
並不能像 ComponentSystem 一樣直接取得 EntityQueryBuilder 來匹配 Entity
而是需要透過 IJob、IJobForEach、IJobChunk...等方式來存取 Component 數據
  
  ```c#
[BurstCompile]  
private struct TranslationJob : IJobForEach<Translation>   
{  
　　public float deltaTime;  
　　public void Execute(ref Translation c0)  
　　{  
　　　　c0.Value.y += deltaTime;  
　　}  
} 
```

## Archetype

<p align="center">
<img style="margin:auto;"  src="https://github.com/ted10401/ECS-Practice/blob/master/GithubResources/unity_ecs_archetype.png">
</p>

***

Archetype 是 Unity ECS 特有的資料結構及概念  
  
在 ECS 架構中  
一個 Entity 具有什麼特性  
完全取決於它掛載了什麼 Component  
複數 Component 組合起來就成為一種能力組  
  
ArcheType 主要權責就是管理這唯一的能力組  
透過它能夠快速訪問擁有此能力組的 Entity  
換句話說在 ECS 架構中  
只要 Entity 內有包含 Component 就一定代表它隸屬於某個 Archetype  

***

### 如何創建 Archetype?

EntityManager.CreateArchetype (params ComponentType[] types);

***

### 如何用 Archetype 創建 Entity?

EntityManager.CreateEntity (EntityArchetype archetype);

## Chunk

<p align="center">
<img style="margin:auto;"  src="https://github.com/ted10401/ECS-Practice/blob/master/GithubResources/unity_ecs_chunk.png">
</p>

***

Chunk 與 Archetype 一樣都是 Unity ECS 特有的資料結構及概念   
由於 Entity 只含有識別實體的 ID  
如何高效的匹配 Entity 就是 Chunk 的作用  
  
Chunk 的本質是固定大小為 16KB 的內存塊  
每一個相連的 Chunk 都具有相同的內存佈局  
而 Entity 在添加或刪除 Component 時會使內存佈局改變  
  
當一個相同特性的 Entity 產生時  
若 Chunk 剩餘大小足夠，則會將 Component 的數據直接放置  
若 Chunk 剩餘大小不足，則會產生一個新的 Chunk 並將它鏈接再一起  
