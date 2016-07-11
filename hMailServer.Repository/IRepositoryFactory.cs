﻿namespace hMailServer.Repository
{
   public interface IRepositoryFactory
   {
      IAccountRepository CreateAccountRepository(string connectionString);
   }
}
