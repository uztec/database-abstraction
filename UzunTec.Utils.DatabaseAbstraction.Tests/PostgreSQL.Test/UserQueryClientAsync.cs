using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UzunTec.Utils.DatabaseAbstraction.PostgreSQL.Test
{
    public class UserQueryClientAsync
    {
        private readonly DBUserAsync dbUser;

        public UserQueryClientAsync(DBUserAsync dbUser)
        {
            this.dbUser = dbUser;
        }

        public Task<bool> Insert(User user)
        {
            return this.dbUser.InsertAsync(user.UserCode, user.UserName, user.UserCodRef, user.PasswordMd5, user.InputDate, user.Status);
        }

        public async Task<bool> Delete(params int[] userCodes)
        {
            bool output = true;
            foreach (int userCode in userCodes)
            {
                output &= await this.Delete(userCode).ConfigureAwait(false);
            }

            return output;
        }

        public Task<bool> Delete(int userCode)
        {
            return this.dbUser.DeleteAsync(userCode);
        }

        public async Task<bool> Update(int oldCode, User user)
        {
            int recordCount = await this.dbUser.UpdateAsync(oldCode, user.UserCode, user.UserName, user.UserCodRef, user.PasswordMd5, user.InputDate, user.Status);
            return recordCount > 0;
        }

        public async Task<User> FindByID(int ID)
        {
            return this.BuildObjectFromRecord(await this.dbUser.FindByIDAsync(ID).ConfigureAwait(false));
        }

        public async Task<User> FindByCode(int userCode)
        {
            return this.BuildObjectFromRecord(await this.dbUser.FindByCodeAsync(userCode).ConfigureAwait(false));
        }

        public async Task<List<User>> ListAll()
        {
            DataResultTable dt = await this.dbUser.ListAllAsync().ConfigureAwait(false);
            return dt.BuildList(this.BuildObjectFromRecord);
        }

        private User BuildObjectFromRecord(DataResultRecord dr)
        {
            return (dr == null) ? null : new User
            {
                UserCode = dr.GetValue<int>("COD_USER"),
                UserName = dr.GetString("USER_NAME"),
                UserCodRef = dr.GetNullableValue<long>("COD_USER_REF"),
                PasswordMd5 = dr.GetString("PASSWORD_MD5"),
                InputDate = dr.GetValue<DateTime>("INPUT_DATE"),
                Status = dr.GetEnum<StatusUser>("USER_STATUS")
            };
        }
    }
}
