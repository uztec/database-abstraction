using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UzunTec.Utils.Common;
using Xunit;

namespace UzunTec.Utils.DatabaseAbstraction.Test
{
    public class DbAbstractionTestList
    {
        private readonly UserQueryClient client;

        public DbAbstractionTestList()
        {
            this.client = DbAbstractionTestContainer.INSTANCE.GetInstance<UserQueryClient>();
        }

        [Fact]
        public void InsertUserWithCodRefTest()
        {
            // Removing users
            this.client.Delete(21,22,23);

            Dictionary<int, User> insertedList = new Dictionary<int, User>();
            insertedList.Add(21, new User
            {
                UserCode = 21,
                UserCodRef = 2333953423432,
                UserName = "Test User1",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("anything"),
                Status = StatusUser.Guest,
            });

            insertedList.Add(22, new User
            {
                UserCode = 22,
                UserName = "Test User2",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("anything-else"),
                Status = StatusUser.Admin,
            });

            insertedList.Add(23, new User
            {
                UserCode = 23,
                UserName = "Test User 3",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("otherthing"),
                Status = StatusUser.User,
            });


            foreach (User user in insertedList.Values)
            {
                Assert.True(this.client.Insert(user));
            }

            List<User> users = this.client.ListAll();

            Assert.NotNull(users);
            Assert.True(users.Count > 2);

            foreach (int cod in insertedList.Keys)
            {
                User user = users.Find(delegate (User u) { return u.UserCode == cod; });
                AssertExt.UsersTheSame(user, insertedList[cod]);
            }

            foreach (int cod in insertedList.Keys)
            {
                Assert.True(this.client.Delete(cod));
            }
        }


        [Fact]
        public void InsertUserMultTasks()
        {
            // Removing users
            this.client.Delete(101, 102);

            Dictionary<int, User> insertedList = new Dictionary<int, User>();
            insertedList.Add(101, new User
            {
                UserCode = 101,
                UserCodRef = 2333953423432,
                UserName = "Test User1",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("anything"),
                Status = StatusUser.Guest,
            });

            insertedList.Add(102, new User
            {
                UserCode = 102,
                UserName = "Test User2",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("anything-else"),
                Status = StatusUser.Admin,
            });

            
            foreach (User user in insertedList.Values)
            {
                 Assert.True(this.client.Insert(user));
            }

            
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 4; i++)
            {
                tasks.Add(Task.Run(() => this.client.ListAll()));
            }

            List<User> users = this.client.ListAll();
            Task.WaitAll(tasks.ToArray());
            Assert.NotNull(users);
            Assert.True(users.Count > 1);

            foreach (int cod in insertedList.Keys)
            {
                User user = users.Find(delegate (User u) { return u.UserCode == cod; });
                AssertExt.UsersTheSame(user, insertedList[cod]);
            }

            foreach (int cod in insertedList.Keys)
            {
                Assert.True(this.client.Delete(cod));
            }
        }
    }
}
