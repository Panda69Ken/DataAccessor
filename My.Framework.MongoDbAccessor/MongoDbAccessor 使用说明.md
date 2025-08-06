## 数据访问组件 MongoDbAccessor 使用说明

##### 使用到的实体定义

```c#
    public class TestEntity
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public List<Address> Addresses { get; set; }  //通过设计主从表，来减低mongodb事务性较差的缺点
    }

    //子表  
    public class Address
    {
        public string City { get; set; }

        public string Street { get; set; }
    }
```



#### 1、单一新增操作（无异常表示操作成功）

```c#
            var entity=new TestEntity()
            {
                Id = SGUID.GenerateComb().ToString(),
                Name = "name",
                Remark = "remark",
                Addresses = new List<Address>()
                {
                    new Address()
                    {
                        City = "广州市",
                        Street = "天园街"
                    },
                    new Address()
                    {
                        City = "广州市",
                        Street = "棠下街"
                    }

                }
            };
            MongoService.Add(entity);//这里的操作是具有原子性的，要么成功，要么失败，不保存即成功
```

#### 2、批量新增操作（无异常表示操作成功）

```c#
            var lst=new List<TestEntity>()
            {
                new TestEntity()
                {
                    Id = SGUID.GenerateComb().ToString(),
                    Name = "name",
                    Remark = "remark",
                    Addresses = new List<Address>()
                    {
                        new Address()
                        {
                            City = "广州市",
                            Street = "天园街"
                        },
                        new Address()
                        {
                            City = "广州市",
                            Street = "棠下街"
                        }

                    }
                },
                new TestEntity()
                {
                    Id = SGUID.GenerateComb().ToString(),
                    Name = "name",
                    Remark = "remark",
                    Addresses = new List<Address>()
                    {
                        new Address()
                        {
                            City = "深圳市",
                            Street = "罗湖街"
                        },
                        new Address()
                        {
                            City = "深圳市",
                            Street = "福永街"
                        }
                    }
                }
            };
            MongoService.AddRange(lst);//批量新增能保证，整体成功或失败
```

#### 3、Mongodb的更新操作提供了两种方法：1、直接再原有文档基础只上对文档个别熟悉进行更新操作；2、直接将旧文档删除掉，添加新文档；两种方法按实际业务场景选择使用；两种方法均返回受影响的记录数

##### 方法1演示如下：仅会更新Name,Remark两个属性

```c#
MongoService.Update(() => new TestEntity()
    {
       Name = "name1",
       Remark = "remark1"
    }, x => x.Id == "1a56652c-e1dc-44a5-d048-baeb1223b160");
```

##### 方法2演示如下：会将旧文档删除，重新插入新文档

```c#
            var count = MongoService.Update(new TestEntity()
            {
                Id = "1a56652c-e1dc-44a5-d048-baeb1223b160",
                Name = "name",
                Remark = "remark",
                Addresses = new List<Address>()
                {
                    new Address()
                    {
                        City = "广州市",
                        Street = "天园街1"
                    },
                    new Address()
                    {
                        City = "广州市",
                        Street = "棠下街1"
                    }

                }
            }, x => x.Id);
```

#### 4、Mongodb的删除操作也提供了两种方法：1、直接通过Expression表达式来进行删除；2、根据实体指定的属性进行删除。两种方法均返回受影响的记录数

##### 方法1演示如下：

```c#
var cityList=new List<string>()
{
      "广州市"
};
var deleteCount = MongoService.Delete<TestEntity>(x => x.Addresses.Any(c => cityList.Contains(c.City)));
```

##### 方法2演示如下：

```c#
var entity=new TestEntity()
    {
        Id = "1a566573-63ea-1449-c2f4-e41c396ea25c"
    };

var deleteCount = MongoService.Delete<TestEntity>(entity, x => x.Id);
```

#### 5、按条件将指定对象做自增操作（返回受影响的记录数）

##### 使用场景：一般应用于没操作一次实体，相关计数器+n的情况，例如：点赞等

```c#
var incCount = MongoService.UpdateInc(() => new TestEntity()
    {
         ViewCount = 1 //自增步长
    }, x => x.Id == "1a5665f4-5959-818c-b4d6-78cca4bcb65f");//该方法能保证高并发下保证操作原子性
```

#### 6、找到一个单一的文件，并原子化更新（returnDocumentAfter 为true返回更新后文档,returnDocumentAfter为false返回更新前文档）

```c#
            var newEntity = MongoService.FindOneAndUpdate(() => new TestEntity()
            {
                Name = "name1"
            }, x => x.Id == "1a5665f4-5959-818c-b4d6-78cca4bcb65f", () => new TestEntity
            {
                ViewCount = 2
            });
```

#### 7.查询操作

##### 可以通过Expression表达式，查询出需要的结果集

```c#
var query = MongoService.GetQueryable<TestEntity>().Where(x => x.Name == "name1");
query = query.Where(x => x.Id == "1a5665f4-5959-818c-b4d6-78cca4bcb65f");
var lst = query.ToList();
```



### 上面使用到的所有方法，均提供异步操作，使用场景和用法是完全相同的！