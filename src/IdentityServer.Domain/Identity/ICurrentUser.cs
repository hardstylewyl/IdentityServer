﻿using System;

namespace IdentityServer.Domain.Identity;

public interface ICurrentUser
{
	bool IsAuthenticated { get; }

	Guid UserId { get; }
}
