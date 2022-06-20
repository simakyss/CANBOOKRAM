namespace CANBOOKRAM.Models
{
    public class UserRating
    {
        public long Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser WhoRated { get; set; }
        public DateTime DateTime { get; set; }
        public int Rate { get; set; } // from 1 to 5
    }

    public class CalculateRate
    {
        public static double GetRating(List<int> rates)
        {
            int star5 = rates.Count(x => x == 5);
            int star4 = rates.Count(x => x == 4);
            int star3 = rates.Count(x => x == 3);
            int star2 = rates.Count(x => x == 2);
            int star1 = rates.Count(x => x == 1);

            double rating = (double)(5 * star5 + 4 * star4 + 3 * star3 + 2 * star2 + 1 * star1) / (star1 + star2 + star3 + star4 + star5);

            rating = Math.Round(rating, 1);

            return rating;
        }
    }
}
