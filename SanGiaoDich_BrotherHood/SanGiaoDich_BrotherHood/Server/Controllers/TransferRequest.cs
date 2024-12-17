namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    internal class TransferRequest
    {
        public int Amount { get; set; }
        public string Description { get; set; }
        public string ReceiverAccount { get; set; }
        public string ReceiverName { get; set; }
    }
}