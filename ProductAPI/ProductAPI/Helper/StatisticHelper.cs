namespace ProductAPI.Helper
{
    public class StatisticHelper
    {

        public string GrowthView(decimal input)
        {
            if (input > 0)
                return "+"+input;
            else return "-"+input;
        }

        public string GrowthClassView(decimal input)
        {
            if (input > 0)
                return "success";
            else return "danger";
        }
    }
}
