using System;
using System.Collections.Generic;
using System.Text;

namespace TejiLib {

    public enum MessageType : byte {
        LoginPhase1,
        LoginPhase2,
        LoginPhase3,
        LoginPhase4,
        Text,
        Command,
        Response,
        Broadcast,
        FileHead,
        FileBody,
        E2E
    }

    [Flags]
    public enum RoomPermission : int {
        HostR = 0b100_000_000,
        HostW = 0b010_000_000,
        HostX = 0b001_000_000,
        MemberR = 0b000_100_000,
        MemberW = 0b000_010_000,
        MemberX = 0b000_001_000,
        OthersR = 0b000_000_100,
        OthersW = 0b000_000_010,
        OthersX = 0b000_000_001,
        NoPermission = 0b000_000_000
    }

}
