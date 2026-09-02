
namespace ConferenceRoomBookingService.Helpers
{
    public class PricingRules
    {
        //These are time-dependent price modifiers.
        public static List<(TimeOnly Start, TimeOnly End, decimal Modifier)> Modifiers = new()
        {
            (new TimeOnly(06, 0), new TimeOnly(09, 0), -0.10m),
            /*Here, you need to place the 12:00–14:00(Peak)
              interval above the 9:00–18:00(standart) interval
              so that the peak time is checked first, followed by the standard time.*/
            (new TimeOnly(12, 0), new TimeOnly(14, 0), 0.15m),
            (new TimeOnly(9, 0), new TimeOnly(18, 0), 0.0m),
            (new TimeOnly(18, 0), new TimeOnly(23, 0), -0.20m)
        };
    }
}
