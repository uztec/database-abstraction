using System;
using UzunTec.Utils.Common;
using Xunit;

namespace UzunTec.Utils.DatabaseAbstraction.Test
{
    public class DbAbstractionTestInsert
    {
        private readonly UserQueryClient client;

        public DbAbstractionTestInsert()
        {
            this.client = DbAbstractionTestContainer.INSTANCE.GetInstance<UserQueryClient>();
        }

        [Fact]
        public void InsertUserWithCodRefTest()
        {
            this.client.Delete(91);
            User userToInsert = new User
            {
                UserCode = 91,
                UserCodRef = 423423423432,
                UserName = "Test User1",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("anything"),
                Status = StatusUser.User,
            };

            client.Delete(userToInsert.UserCode);  // Avoid Duplicates

            Assert.True(client.Insert(userToInsert));
            User insertedUser = client.FindByCode(userToInsert.UserCode);
            Assert.NotNull(insertedUser);
            AssertExt.UsersTheSame(userToInsert, insertedUser);
            Assert.True(client.Delete(insertedUser.UserCode));
        }


        [Fact]
        public void InsertUserWithoutCodRefTest()
        {
            this.client.Delete(92);
            User userToInsert = new User
            {
                UserCode = 92,
                UserName = "Test User2",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("anything-else"),
                Status = StatusUser.Guest,
            };

            client.Delete(userToInsert.UserCode);  // Avoid Duplicates

            Assert.True(client.Insert(userToInsert));
            User insertedUser = client.FindByCode(userToInsert.UserCode);
            Assert.NotNull(insertedUser);
            AssertExt.UsersTheSame(insertedUser, userToInsert);
            Assert.True(client.Delete(insertedUser.UserCode));
        }
    }
}
