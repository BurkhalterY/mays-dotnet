using System.Threading.Tasks;
using Epsic.Info3e.Mays.Models;

namespace Epsic.Info3e.Mays.Services
{
    public interface IPremiumService
    {
        /// <summary>
        /// Makes an user premium
        /// </summary>
        /// <param name="userId">User to make premium</param>
        /// <param name="creditCard">Credit card details</param>
        /// <returns>True if the user is now premium, false otherwise</returns>
        public Task<bool> MakePremiumAsync(string userId, CreditCard creditCard);

        /// <summary>
        /// Makes an user no longer premium and cancels the auto renew
        /// </summary>
        /// <param name="userId">Id of the user to un premium</param>
        public Task CancelSubscriptionAsync(string userId);
    }
}
