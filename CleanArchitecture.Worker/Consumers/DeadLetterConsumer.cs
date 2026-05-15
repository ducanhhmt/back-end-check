//using CleanArchitecture.Messages.Messages;
//using MassTransit;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CleanArchitecture.Worker.Consumers
//{
//    public class DeadLetterConsumer : IConsumer<ExpiredVerificationMessage>
//    {
//        public async Task Consume(ConsumeContext<ExpiredVerificationMessage> context)
//        {
//            var msg = context.Message;
//            Console.WriteLine($"[EXPIRE] Xóa tài khoản với (UserID: {msg.UserId}) vì hết hạn xác thực.");

//            // Bắn message cho WebApi xóa user thật
//            await context.Publish(new DeleteUserMessage(msg.UserId));
//        }
//    }
//}
