using System;
using System.Collections.Generic;

namespace UzunTec.Utils.DatabaseAbstraction.SQLServer.Test
{
    public class UserQueryClient
    {
        private readonly DBUser dbUser;

        public UserQueryClient(DBUser dbUser)
        {
            this.dbUser = dbUser;
        }

        public bool Insert(User user)
        {
            return this.dbUser.Insert(user.UserCode, user.UserName, user.UserCodRef, user.PasswordMd5, user.InputDate, user.Status);
        }

        public bool Delete(params int[] userCodes)
        {
            bool output = true;
            foreach (int userCode in userCodes)
            {
                output &= this.Delete(userCode);
            }

            return output;
        }

        public bool Delete(int userCode)
        {
            return this.dbUser.Delete(userCode);
        }

        public bool Update(int oldCode, User user)
        {
            return this.dbUser.Update(oldCode, user.UserCode, user.UserName, user.UserCodRef, user.PasswordMd5, user.InputDate, user.Status) > 0;
        }

        public User FindByID(int ID)
        {
            return this.BuildObjectFromRecord(this.dbUser.FindByID(ID));
        }

        public User FindByCode(int userCode)
        {
            return this.BuildObjectFromRecord(this.dbUser.FindByCode(userCode));
        }

        public List<User> ListAll()
        {
            return this.dbUser.ListAll().BuildList(this.BuildObjectFromRecord);
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
