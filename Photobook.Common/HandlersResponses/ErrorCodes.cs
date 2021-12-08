using System;

namespace Photobook.Common.HandlersResponses
{
    public static class ErrorCodes
    {
        public static readonly Guid EmailAlreadyExists = Guid.Parse("ef8b691d-fa26-4658-8174-1a8667241c1a");
        public static readonly Guid InvalidToken = Guid.Parse("6dbb03d8-6946-4a6e-935b-3158177be7fe");
        public static readonly Guid UnexpectedError = Guid.Parse("3442264b-e34b-4d5c-af1e-5aef42913ec0");
        public static readonly Guid InvalidUser = Guid.Parse("579dc57d-08cf-4474-8675-d8300fc23eee");
        public static readonly Guid UserNotActive = Guid.Parse("9c5472dc-c6e7-430a-88d5-bc7c9b9ad4c4");
        public static readonly Guid UserAlreadyRegister = Guid.Parse("3ce2ff56-85d2-4dda-8503-84754885989d");
    }
}
