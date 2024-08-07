/*
 Copyright (C) 2015 - 2020 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using Orbit.Client.Addressable;
using Orbit.Shared.Addressable;

namespace Orbit.Client.Execution;

public interface IDeactivatable
{
    AddressableReference Reference { get; }
    Task Deactivate(DeactivationReason deactivationReason);
}