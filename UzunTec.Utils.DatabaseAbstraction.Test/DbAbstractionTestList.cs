using System;
using System.Collections.Generic;
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
            Dictionary<int, User> insertedList = new Dictionary<int, User>();
            insertedList.Add(21, new User
            {
                UserCode = 21,
                UserCodRef = 2333953423432,
                UserName = "Test User1",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("anything"),
            });

            insertedList.Add(22, new User
            {
                UserCode = 22,
                UserName = "Test User2",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("anything-else"),
            });

            insertedList.Add(23, new User
            {
                UserCode = 23,
                UserName = "Test User 3",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("otherthing"),
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
               

    }
}
