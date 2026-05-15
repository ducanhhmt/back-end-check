using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Messages.Messages
{
    public record SendVerificationCodeMessage(Guid UserId, string VerificationCode);

    public record ActivateUserMessage(Guid UserId);

    public record DeleteUserMessage(Guid UserId);

    public record ExpiredVerificationMessage(Guid UserId);
}
