
using CleanArchitecture.Messages.Messages;
using MassTransit;

namespace CleanArchitecture.Worker.Consumers
{
    public class VerificationConsumer : IConsumer<SendVerificationCodeMessage>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        public VerificationConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<SendVerificationCodeMessage> context)
        {
            var msg = context.Message;
            // Nếu sai hoặc không nhập → bắn delayed message xóa sau 1 phút
            //await _publishEndpoint.Publish(new ExpiredVerificationMessage(msg.UserId), ctx =>
            //{
            //    ctx.TimeToLive = TimeSpan.FromMinutes(1); // TTL 1 phút trên message
            //});
            Console.WriteLine($"[GET MESSAGE] GENERATE FROM UserID: {msg.UserId})");
            Console.WriteLine($"YOUR VERIFY CODE IS: {msg.VerificationCode}");
            Console.Write("PLEASE ENTER YOUR VERIFY CODE: ");
            var inputCode = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(inputCode))
            {
                Console.WriteLine("[WARNING] AFTER 1 MINUTE , IF U DONT WRITE VERIFY CODE , UR ACCOUNT WILL BE DELETED");
            }
            else if (inputCode == msg.VerificationCode)
            {
                Console.WriteLine("[CORRECT] TIME TO ACTIVE USER");
                // Bắn message cho API kích hoạt user
                await _publishEndpoint.Publish(new ActivateUserMessage(msg.UserId));
                Console.WriteLine($"[DONE] User {msg.UserId} HAS BEEN ACTIVED");
                return;
            }
            else
            {
               
                // Sai hoặc không nhập → bắn message delay vào queue riêng
                await _publishEndpoint.Publish(new DeleteUserMessage(msg.UserId), ctx =>
                {
                    // Delay 1 phút – dùng x-delay header
                    ctx.TimeToLive = TimeSpan.FromMinutes(2);
                    //ctx.SetRoutingKey("delete-pending-delay-queue"); // Đẩy vào queue delay riêng
                });
                Console.WriteLine("[ERROR] WRONG VERIFY CODE! ACCOUNT WILL BE DELETED AFTER 1MIN IF U DONT ENTER CORRECT VERIFY CODE");
            }
           
        }
    }
}
