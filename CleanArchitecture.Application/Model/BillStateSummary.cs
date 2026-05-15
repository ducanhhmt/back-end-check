namespace CleanArchitecture.Application.Model
{
    public class BillStateSummary
    {
        public int All { get; set; } = 0;
        public int PaymentWaiting { get; set; } = 0; //state = 4
        public int Processing { get; set; } = 0; // state = 0
        public int Shipping { get; set; } = 0; // state = 1
        public int Completed { get; set; } = 0;// state = 2
        public int Cancelled { get; set; } = 0;// state = 3
    }
}
