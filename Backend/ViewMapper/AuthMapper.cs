using System;
using CoinTracker_Backend.ViewModels;
using CoinTracker_Backend.Models;
namespace CoinTracker_Backend.ViewMapper
{
	public class AuthMapper
	{
		public static UserModel MapRegisterViewModelToUserModel(RegisterViewModel model)
		{
            return new UserModel()
            {
                Email = model.Email!,
                Password = model.Password!
            };
        }
    }
}

