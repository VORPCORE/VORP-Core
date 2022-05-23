exports('DisplayLeftNotification', (title, subTitle, dict, icon, duration) => {
    const struct1 = new DataView(new ArrayBuffer(48));
    struct1.setInt32(0, duration, true);

    const string1 = CreateVarString(10, "LITERAL_STRING", title);
    const string2 = CreateVarString(10, "LITERAL_STRING", subTitle);

    const struct2 = new DataView(new ArrayBuffer(56));
    struct2.setBigInt64(8, BigInt(string1), true);
    struct2.setBigInt64(16, BigInt(string2), true);
    struct2.setBigInt64(32, BigInt(GetHashKey(dict)), true);
    struct2.setBigInt64(40, BigInt(GetHashKey(icon)), true);
    struct2.setBigInt64(48, BigInt(GetHashKey("COLOR_WHITE")), true);


    Citizen.invokeNative("0x26E87218390E6729", struct1, struct2, 1, 1);
});

exports('DisplayTopCenterNotification', (text, location, duration) => {
    const string = CreateVarString(10, "LITERAL_STRING", location);
    const string2 = CreateVarString(10, "LITERAL_STRING", text);

    const struct1 = new DataView(new ArrayBuffer(48));
    struct1.setInt32(0, duration, true);

    const struct2 = new DataView(new ArrayBuffer(24));
    struct2.setBigInt64(8, BigInt(string), true);
    struct2.setBigInt64(16, BigInt(string2), true);
    Citizen.invokeNative("0xD05590C1AB38F068", struct1, struct2, 1, 1);
});

exports('DisplayTip', (text, duration) => {
    const str = CreateVarString(10, "LITERAL_STRING", text);

    const struct1 = new DataView(new ArrayBuffer(48));
    struct1.setUint32(0, duration, true);

    const struct2 = new DataView(new ArrayBuffer(8 + 8));
    struct2.setBigUint64(8, BigInt(str), true);

    Citizen.invokeNative("0x049D5C615BD38BAD", struct1, struct2, 1);

});

exports('DisplayRightTip', (text, duration) => {
    // int32 x4
    const string = CreateVarString(10, "LITERAL_STRING", text);
    const struct1 = new DataView(new ArrayBuffer(48));
    struct1.setInt32(0, duration, true); // duration

    // int32 (weird padding) + int32 + uint64
    const struct2 = new DataView(new ArrayBuffer((4 * 2) + 8));
    struct2.setBigInt64(8, BigInt(string), true);
    Citizen.invokeNative("0xB2920B9760F0F36B", struct1, struct2, 1);
});

exports('DisplayObjetive', (text, duration) => {
    // int32 x4
    //Citizen.invokeNative("0xA3108D6981A5CADB");
    Citizen.invokeNative("0xDD1232B332CBB9E7", 3, 1, 0);
    const string = CreateVarString(10, "LITERAL_STRING", text);
    const struct1 = new DataView(new ArrayBuffer(48));
    struct1.setInt32(0, duration, true); // duration 

    // int32 (weird padding) + int32 + uint64
    const struct2 = new DataView(new ArrayBuffer((4 * 2) + 8));
    struct2.setBigInt64(8, BigInt(string), true);
    Citizen.invokeNative("0xCEDBF17EFCC0E4A4", struct1, struct2, 1);
});