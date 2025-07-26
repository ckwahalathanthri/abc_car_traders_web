using Microsoft.AspNetCore.Http;

namespace ABCCarTraders.Helpers
{
    public static class ViewSessionExtensions
    {
        public static int GetWishlistCount(this ISession session)
        {
            var wishlistCountString = session.GetString("WishlistCount");
            return int.TryParse(wishlistCountString, out int wishlistCount) ? wishlistCount : 0;
        }
    }
}