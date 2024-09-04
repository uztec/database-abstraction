using System;
using System.Threading.Tasks;
using UzunTec.Utils.Common;
using Xunit;

namespace UzunTec.Utils.DatabaseAbstraction.SQLServer.Test
{
    [Collection("BootstrapCollectionFixture")]
    public class DbAbstractionTestInsertAsync
    {
        private readonly UserQueryClientAsync client;

        public DbAbstractionTestInsertAsync(BootstrapFixture bootstrap)
        {
            this.client = bootstrap.GetInstance<UserQueryClientAsync>();
        }

        [Fact]
        public async Task InsertUserWithCodRefTest()
        {
            await this.client.Delete(91);
            User userToInsert = new User
            {
                UserCode = 91,
                UserCodRef = 423423423432,
                UserName = "Test User1",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("anything"),
                Status = StatusUser.User,
            };

            await this.client.Delete(userToInsert.UserCode);  // Avoid Duplicates

            Assert.True(await this.client.Insert(userToInsert));
            User insertedUser = await this.client.FindByCode(userToInsert.UserCode);
            Assert.NotNull(insertedUser);
            AssertExt.UsersTheSame(userToInsert, insertedUser);
            Assert.True(await this.client.Delete(insertedUser.UserCode));
        }


        [Fact]
        public async Task InsertUserWithoutCodRefTest()
        {
            await this.client.Delete(92);
            User userToInsert = new User
            {
                UserCode = 92,
                UserName = "Test User2",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("anything-else"),
                Status = StatusUser.Guest,
            };

            await this.client.Delete(userToInsert.UserCode);  // Avoid Duplicates

            Assert.True(await this.client.Insert(userToInsert));
            User insertedUser = await this.client.FindByCode(userToInsert.UserCode);
            Assert.NotNull(insertedUser);
            AssertExt.UsersTheSame(insertedUser, userToInsert);
            Assert.True(await this.client.Delete(insertedUser.UserCode));
        }
    }
}
