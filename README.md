# ECS-Practice

## Entities

<p align="center">
<img style="margin:auto;"  src="https://github.com/ted10401/ECS-Practice/blob/master/GithubResources/unity_ecs_entities.png">
</p>

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
