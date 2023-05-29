using FluentResults;
using System;

namespace MainCore.Helper.Interface
{
    public interface IAdventureHelper
    {
        Result StartAdventure(int accountId);

        DateTime GetAdventureTimeLength(int accountId);

        Result ToAdventure(int accountId);
    }
}