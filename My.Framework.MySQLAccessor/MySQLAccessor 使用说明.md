##数据访问组件 `MySQLAccessor` 使用说明

**重要说明：**

数据访问组件基于Dapper进行了二次封装，所以会默认遵循Dapper的相关规范；属性名与表列名的对应，除了遵循几个特性`KeyAttribute`,`ExplicitKeyAttribute`,`WriteAttribute`,`ExtentionAttribute`外,仍默认遵循形如属性`UserName`对应`user_name`列名的准则。数据访问相关方法均提供了同步与异步两种操作方式，下面的实例仅对同步方法举例说明，同样适用于异步方法（同步方法名+Async） 

高阶查询部分的`Where`方法传入的`Expression` 支持 `==`，`>`,`>=`,`<`,`<=`等，若类型为`int`或`string` 还支持`Equals`方法，类型为`List<int>`或`List<string>`支持`Contains`方法,对应生成Sql语句为`xx in( ……)`类型为`string`支持 `StartsWith`,`EndWith`,`Contains` 分别对应生成Sql为`xx like 'xx%'`,`xxx like '%xx'`,`xx like '%xx%'`其中后两个方法要慎用会造成全表扫描，多个`Expression`拼接支持`&&`, `||`

### 一 、基本用法

**说明：**原生Dapper支持功能，通过SQL语句进行相关查询，性能也最高，接近于ADO.NET

#### 1）执行查询并将结果映射到强类型列表

**例子如下**

```c#
  public class DemoTest
    {
        public int Id { get; set; }
        
        public string DemoName { get; set; }

        public string DemoRemark { get; set; }

        public string DemoTrs { get; set; }

        public string Address { get; set; }

        public DateTime? Birthday { get; set; }
    }

    [Fact]
    public void QueryListExample()
        {
            var querySql = @"SELECT
	                    id,
	                    demo_name,
	                    demo_remark,
	                    demo_trs,
	                    address,
	                    birthday 
                    FROM
	                    demo_test where id=@id";
            var id = 1606;
            var lst = MySqlContext.Query<DemoTest>(querySql, new {id}).ToList();

            Assert.Single(lst);
            Assert.Equal(id,lst.FirstOrDefault().Id);
        }
```

#### 2)执行SQL，返回单个值

**例子如下：**

```c#
        [Fact]
        public void ExecuteScalarExample()
        {
            var querySql = "select user_name from userinfo where user_id=@userId";

            var userId = 1;
            var name = MySqlContext.ExecuteScalar<string>(querySql, new {userId});
            Assert.Equal("2211",name);
        }
```



#### 3) 执行查询并将其映射到动态对象列表

**例子如下：**

```c#
        [Fact]
        public void QueryDynamicExample()
        {
            var c = @"SELECT
	                    id,
	                    demo_name,
	                    demo_remark,
	                    demo_trs
                    FROM
	                    demo_test where id=@id";
            var id = 1606;
            var rows = MySqlContext.Query<dynamic>(querySql, new { id }).ToList();

            Assert.Single(rows);
            Assert.Equal(id, Convert.ToInt32(rows[0].id));

        }
```

#### 4) 执行不返回结果的命令

**例子如下：**

```C#
        [Fact]
        public void ExcuteCmdNoResultExample()
        {
            var cmd = @"INSERT INTO demo_test 
                        ( demo_name, demo_remark, demo_trs, address, birthday )
                        VALUES
                       (@demoname, @demoremark, @demotrs, @address, @birthday)";
            var demoTest=new DemoTest()
            {
                DemoName = "demoname",
                DemoRemark = "demoremark",
                DemoTrs = "demotrs",
                Address = "address",
                Birthday = DateTime.Now
            };
            var count = MySqlContext.Execute(cmd, demoTest);
            Assert.Equal(1,count);
        }

        [Fact]
        public void ExcuteCmdNoResultExample2()
        {
            var cmd = @"INSERT INTO demo_test 
                        ( demo_name, demo_remark, demo_trs, address, birthday )
                        VALUES
                       (@name, @remark, @trs, @address, @birthday)";
            var count = MySqlContext.Execute(cmd, new
            {
                Name="name",
                Remark="remark",
                Trs="trs",
                Address="address",
                Birthday=DateTime.Now
            });
            Assert.Equal(1, count);
        }	
```

#### 5) 多次执行命令

**例子如下：**

```c#
        [Fact]
        public void ExecuteCmdMultiple()
        {
            var cmd = @"INSERT INTO demo_test 
                        ( demo_name, demo_remark, demo_trs, address, birthday )
                        VALUES
                       (@demoname, @demoremark, @demotrs, @address, @birthday)";
            var lst = new List<DemoTest>()
            {
                new DemoTest()
                {
                    DemoName = "demoname1",
                    DemoRemark = "demoremark1",
                    DemoTrs = "demotrs1",
                    Address = "address1",
                    Birthday = DateTime.Now
                },
                new DemoTest()
                {
                    DemoName = "demoname2",
                    DemoRemark = "demoremark2",
                    DemoTrs = "demotrs2",
                    Address = "address2",
                    Birthday = DateTime.Now
                },
                new DemoTest()
                {
                    DemoName = "demoname2",
                    DemoRemark = "demoremark2",
                    DemoTrs = "demotrs2",
                    Address = "address2",
                    Birthday = DateTime.Now
                }
            };
            var count = MySqlContext.Execute(cmd, lst);
            
            Assert.Equal(3,count);
        }

        [Fact]
        public void ExecuteCmdMultiple2()
        {
            var cmd = @"INSERT INTO demo_test 
                        ( demo_name, demo_remark, demo_trs, address, birthday )
                        VALUES
                       (@name, @remark, @trs, @address, @birthday)";
            var count = MySqlContext.Execute(cmd, new[]
                {
                    new {
                        Name = "name1",
                        Remark = "remark1",
                        Trs = "trs1",
                        Address = "address1",
                        Birthday = DateTime.Now
                    },
                    new {
                        Name = "name2",
                        Remark = "remark2",
                        Trs = "trs2",
                        Address = "address2",
                        Birthday = DateTime.Now
                    },
                    new {
                        Name = "name3",
                        Remark = "remark3",
                        Trs = "trs3",
                        Address = "address3",
                        Birthday = DateTime.Now
                    }

                }
            );
            Assert.Equal(3, count);
        }
```

#### 6) 集合支持

**例子如下：**

```c#
        [Fact]
        public void ListSupportExample()
        {
            var querySql = @"SELECT * FROM demo_test where id in @ids";
            var lst = MySqlContext.Query<DemoTest>(querySql, new
            {
                Ids = new int[]
                {
                    10001,
                    10002,
                    10003
                }
            }).ToList();

            Assert.Equal(3,lst.Count);
        }
```

#### 7) 多映射

**例子如下：**

```C#
    public class User
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Address { get; set; }

        public Dept Dept { get; set; }
    }

    public class Dept
    {
        public int DeptId { get; set; }

        public string DeptName { get; set; }

        public string Sortcode { get; set; }
    }


        [Fact]
        public void MultiMappingExample()
        {
            var querySql = @"SELECT
	                            * 
                            FROM
	                            userinfo a
	                            INNER JOIN deptinfo b ON a.dept_id = b.dept_id 
                            WHERE
	                            id =@id";

            var id = 3;
            var userList = MySqlContext.Query<User, Dept, User>(querySql, (user, dept) =>
            {
                user.Dept = dept;
                return user;
            }, new {id},"dept_id").ToList();

            Assert.Single(userList);
            Assert.Equal(1,userList.FirstOrDefault().Dept.DeptId);
        }
```

#### 8) 多结果

**例子如下：**

```c#
        [Fact]
        public void MultiResultExample()
        {
            var querySql = @"select * from userinfo where user_id=@userId;
                            select * from deptinfo where dept_id=@deptId;";

            var userId = 1;
            var deptId = 1;
            var result = MySqlContext.QueryMultiple<User, Dept>(querySql, new {userId, deptId});

            Assert.Single(result.Item1);
            Assert.Single(result.Item2);
        }
```



#### 9) 多结果集

**例子如下：**

```c#
        [Fact]
        public void QueryGridReaderExample()
        {
            var querySql = @"select * from userinfo where user_id=@userId;
                            select * from deptinfo where dept_id=@deptId;";

            var userId = 1;
            var deptId = 1;

            using (var conn=MySqlContext.CreateConnection())
            {
                var reader = conn.QueryMultiple(querySql, new {userId, deptId});
                var user = reader.Read<User>().Single();

                var dept = reader.Read<Dept>().Single();

                Assert.Equal(1, user.UserId);
                Assert.Equal(1, dept.DeptId);
            }
        }
```

#### 10) 多结果单一查询

**例子如下：**

```c#
        [Fact]
        public void QueryMultipleSingleExample()
        {
            var querySql = @"select * from userinfo where user_id=@userId;
                            select * from deptinfo where dept_id=@deptId;";

            var userId = 1;
            var deptId = 1;
            var result = MySqlContext.QueryMultipleSingle<User, Dept>(querySql, new { userId, deptId });

            Assert.Equal(1,result.Item1.UserId);
            Assert.Equal(1,result.Item2.DeptId);
        }
```

####11) 执行单一查询

**例子如下：**

```c#
        [Fact]
        public void QuerySingleExample()
        {
            var querySql= "select * from userinfo where user_id=@userId";

            var userId = 1;

            var user = MySqlContext.QuerySingle<User>(querySql, new {userId});

            Assert.Equal(1,user.UserId);
        }
```

### 二、扩展用法

**说明：**扩展用法生成相对应实体的`Insert`,`Update`,`Delete`语句时会进行缓存，所以性能会高于下面的每次都重新生成SQL语句；扩展用法中操作的实体会用到`KeyAttribute`标识自增主键,`ExplicitKeyAttribute`标识非自增主键或联合主键

#### 1) 新增操作

**例子如下：**

```c#
        [Fact]
        public void InsertExample()
        {
            var demoTest=new DemoTest()
            {
                DemoName = "demoname",
                DemoRemark = "demoremark",
                DemoTrs = "demotrs",
                Address = "address",
                Birthday = DateTime.Now
            };
		   //插入操作会返回执行结果，若有标识[Key]也会将自动生成的Id赋值给该属性	
            var result = MySqlContext.Insert(demoTest);

            Assert.True(result);
            Assert.NotEqual(0,demoTest.Id);
        }
```

#### 2）更新操作

**例子如下：**

```c#
        [Fact]
        public void UpdateExample()
        {
            var demoTest = new DemoTest()
            {
                DemoName = "demoname_update",
                DemoRemark = "demoremark_update",
                DemoTrs = "demotrs_update",
                Address = "address_update",
                Birthday = DateTime.Now,
                Id = 10036
            };
            //根据实体中含有[Key]或[ExplicitKey]特性的属性字段进行更新
            var result = MySqlContext.Update(demoTest);
            Assert.True(result);
        }
```

#### 3)删除操作

**例子如下：**

```c#
        [Fact]
        public void DeleteExample()
        {
            var id = 10036;
            var demoTest=new DemoTest()
            {
                Id = id
            };
            //根据实体中含有[Key]或[ExplicitKey]特性的属性字段进行更新
            var result = MySqlContext.Delete(demoTest);
            Assert.True(result);
            var entity = MySqlContext.Get<DemoTest>(id);
            Assert.Null(entity);
        }
```

#### 4)查询操作

**例子如下：**

```c#
        [Fact]
        public void GetQueryExample()
        {
            var id = 10035;
            var entity = MySqlContext.Get<DemoTest>(id);//会通过[Key]或[ExplicitKey]对于的字段进行where条件拼接
            Assert.Equal(id,entity.Id);
        }
```

### 三、高阶查询

####1）一般查询操作

**例子如下：**

```c#
            var demoName = "DemoName11";
            var query = MySqlContext.Query<DemoTest>().Where(x => x.DemoName == demoName);//延迟查询
            var lst = query.ToList();
            Assert.Equal(2,lst.Count());

            var demoId = 1606;
            var lst2= query.Where(x=>x.Id==demoId).ToList();//链式编程
            Assert.Single(lst2);
```

#### 2） 根据需要查询所要返回的列

**应用场景：**有时我们不需要查询一个表的全部列，而会根据业务需要查询某个表的一些列

**例子如下：**

```c#
       var startId = 10000;
       var lst = MySqlContext.Query<DemoTest>().Where(x => x.Id > startId).Select(x => new
            {
                x.Id,
                x.DemoName
            }).ToList();
        var first = lst.FirstOrDefault();
        Assert.Null(first.Address);
        Assert.NotNull(first.DemoName);
```

#### 3) 插入数据

**说明：**此处的插入操作由于未对Sql语句进行缓存，所以性能会低于上述`2.1`的插入操作

**例子如下：**

```c#
            var tt = new DemoTest()
            {
                DemoName = "name01",
                Address = "address01",
                DemoRemark = "demoremark01",
                DemoTrs = "demotrs"
            };
            var result = MySqlContext.Query<DemoTest>().Insert(tt);
            Assert.True(result);//插入操作的返回值
            Assert.Equal(10021,tt.Id);//若主键为自增Id，插入后会将生成的Id赋值给实体的主键[KeyAttribute]属性
			
   			//按需插入指定列
            var result = MySqlContext.Query<DemoTest>().Insert(c=>new DemoTest()
            {
                Address = "222",
                DemoName = "demoname"
            });
            Assert.True(result);
```

#### 4） 按条件更新数据

**说明：**这里的数据更新由于相关SQL语句未进行缓存在性能上也略低于上面`2.2`的更新操作

**例子如下：**

```C#
		   //按条件更新整个实体对象（表）
		   var id = 1605;
            var dt=new DemoTest()
            {
                DemoName = "D1606",
                Address = "A1606",
                DemoTrs = "DT1606",
                DemoRemark = "DR1606",
                Id = 1606
            };
            MySqlContext.Query<DemoTest>().Update(x => x.Id == id, dt);
		
		  //按条件更新指定列
		   var dn = "D1";
            var id = 3213;
            MySqlContext.Query<DemoTest>().Update(x => x.Id == id, c => new DemoTest()
            {
                DemoName = dn,
                Address = "A1",
                DemoTrs = "DT1",
                Birthday = DateTime.Now
            });

```

####5) 按条件删除数据

**说明：**按条件删除数据，返回受影响的行数

**例子如下：**

```c#
            var id = 10014;
            var row = MySqlContext.Query<DemoTest>().Delete(x => x.Id == id);
            Assert.Equal(1, row);
```

####6) 分页查询

**说明：**首先构建查询条件，然后查询出总条数；然后再进行排序和分页

**例子如下：**

```c#
var startId = 10000;
var query = MySqlContext.Query<DemoTest>().Where(x => x.Id > startId);
var totalCount = query.Count();
var result =query.OrderByDescending(x=>x.Id).GetRange(1, 2).ToList();
```

#### 7) Top n查询

**例子如下：**

```C#
var startId = 10000;
var lst =MySqlContext.Query<DemoTest>().Where(x => x.Id > startId).OrderByDescending(x => x.Id).Top(5).ToList();
```

#### 8) 按条件查询，获取第一条数据

**例子如下：**

```c#
var startId = 10000;
var entity = MySqlContext.Query<DemoTest>().Find(x => x.Id > startId).OrderByDescending(x => x.Id).ToEntity();
```

#### 9) Distinct查询

**说明：**按条件查询，获取结果的唯一值

**例子如下：**

```c#
var startId = 10000;
var lst = MySqlContext.Query<DemoTest>().Where(x => x.Id > startId).Distinct(c => c.Address).ToList();
```

#### 10) Sum查询

**说明：**Sum查询用于按条件对某列进行聚合统计

**例子如下：**

```c#
//example 1
var lst = new List<int>()
   {
       10001,
       10002,
       10003,
       10004,
       10005,
       10012,
       10013,
   };
var result = MySqlContext.Query<DemoTest>().Where(x => lst.Contains(x.Id)).Sum(c => c.Id);
//example 2
var lst = new List<string>()
{
      "n10001",
      "n10002",
      "n10003",
      "n10004",
      "n10005"
 };
var result = MySqlContext.Query<DemoTest>().Where(x => lst.Contains(x.DemoName)).Sum(c => c.Id);
//example 3
var result = MySqlContext.Query<DemoTest>().Where(x => x.DemoName.StartsWith("D")).Sum(c => c.Id);
```

#### 11) Max查询

**例子如下：**

```c#
var lst = new List<string>()
 {
     "n10001",
     "n10002",
     "n10003",
     "n10004",
     "n10005"
 };
//查询符合条件的最大的Id
var result = MySqlContext.Query<DemoTest>().Where(x => lst.Contains(x.DemoName)).Max(x => x.Id);
```

#### 12) Count查询

**说明：**统计所需查询的记录数

**例子如下：**

```c#
var endId = 10017;
//统计id小于等于10017的记录数
var count = MySqlContext.Query<DemoTest>().Where(x => x.Id <= endId).Count();
```

