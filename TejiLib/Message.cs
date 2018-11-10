using System;
using System.Collections.Generic;
using System.Text;

namespace TejiLib {

    public enum MessageType : byte {
        /*
        client

        string name
        */
        LoginPhase1,
        /*
        server

        byte[128] salt1
        byte[128] salt2
        */
        LoginPhase2,
        /*
        client 

        byte[] password_encrypted
        */
        LoginPhase3,
        /*
        server

        byte[1] 61_sign
        */
        LoginPhase4,
        /*
        client

        int32 room_length
        string room
        string words
        */
        TextIn,
        /*
        server

        int32 room_length
        string room
        int32 user_length
        string user
        int64 time_stamp_utc
        string words
        */
        TextOut,
        /*
        client

        string commands
        */
        Command,
        /*
        server

        string response_words
        */
        Response,
        /*
        server

        string words
        */
        Broadcast,
        /*
        server / client

        byte[256] guid
        */
        Request,
        /*
        client / server

        byte[256] guid
        int32 section_count
        int32 section_length
        int32 last_section_length
        */
        FileHead,
        /*
        client / server

        byte[256] guid
        int32 section
        byte[] data
        */
        FileBody,
        /*
        client

        int32 to_length
        string to
        byte[] data
        */
        E2EIn,
        /*
        server

        int32 from_length
        string from
        byte[] data
        */
        E2EOut
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
