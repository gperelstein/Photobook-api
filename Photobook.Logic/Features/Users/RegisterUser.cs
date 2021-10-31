using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Photobook.Logic.Features.Users
{
    public class RegisterUser
    {
        public class Command : IRequest
        {
            public string Password { get; set; }
            public string Token { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            public Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
