using ClickableTransparentOverlay;
using ImGuiNET;
using Swed32;

internal class Program : Overlay
{
    private static IntPtr BaseModule = IntPtr.Zero;
    private static IntPtr Money = IntPtr.Zero;
    private static IntPtr Stamina = IntPtr.Zero;
    private static IntPtr CurentSpeed = IntPtr.Zero;
    private static IntPtr TempModule = IntPtr.Zero;
    private static Swed _swed = new Swed("Kingdom");
    private static bool WriteInConsole = false;
    private static void Main(string[] args)
    {
        BaseModule = _swed.GetModuleBase("Kingdom.exe");
        TempModule = _swed.ReadPointer(BaseModule + 0x0EF22E0) + 0x1C;
        TempModule = _swed.ReadPointer(TempModule) + 0xAEC;
        TempModule = _swed.ReadPointer(TempModule) + 0x14;
        TempModule = _swed.ReadPointer(TempModule) + 0x20;
        Money = _swed.ReadPointer(TempModule) + 0x4C;
        Money = _swed.ReadPointer(Money) + 0x3C;
        Stamina = _swed.ReadPointer(TempModule) + 0xC4;
        CurentSpeed = _swed.ReadPointer(TempModule) + 0x54;
        CurentSpeed = _swed.ReadPointer(CurentSpeed) + 0x28;
        Program program = new Program();
        program.Start();
    }

    protected override void Render()
    {
        ImGui.Begin("Kingdom Trainer");
        if (ImGui.Checkbox("Infinity money", ref IsInfinityMoney))
        {
            Task.Run(() => InfinityMoney());
        }
        if (ImGui.Checkbox("Infinity stamina", ref IsInfinityStamina))
        {
            Task.Run(() => InfinityStamina());
        }
        ImGui.Checkbox("SpeedHack enable", ref IsSpeedHackEnable);
        if (IsSpeedHackEnable)
        {
            ImGui.SliderInt("Multiply", ref SpeedHackMultiply, 1, 10);
            Task.Run(() => StartSpeedHack());
        }
    }

    private async Task StartSpeedHack()
    {
        while (IsSpeedHackEnable && SpeedHackMultiply != 1)
        {
            if (_swed.ReadFloat(CurentSpeed) >= 4 || _swed.ReadFloat(CurentSpeed) == 0) { continue; }
            _swed.WriteFloat(CurentSpeed, _swed.ReadFloat(CurentSpeed) * SpeedHackMultiply);
            await Task.Delay(20);
        }
    }

    private static bool IsSpeedHackEnable = false;
    private static int SpeedHackMultiply = 2;
    private async void InfinityStamina()
    {
        while (IsInfinityStamina)
        {
            _swed.WriteFloat(Stamina, 39f);
            await Task.Delay(40);
        }
    }

    private static bool IsInfinityStamina = false;
    private static bool IsInfinityMoney = false;
    private static async Task InfinityMoney()
    {
        while (IsInfinityMoney)
        {
            _swed.WriteInt(Money, 100);
            await Task.Delay(40);
        }
    }

    private static async Task StartLogger()
    {
        while (WriteInConsole)
        {
            Console.WriteLine("Money: " + _swed.ReadInt(Money));
            Console.WriteLine("Stamina: " + _swed.ReadFloat(Stamina));
            Console.WriteLine("Curent speed: " + _swed.ReadFloat(CurentSpeed));
            await Task.Delay(1000);
        }
    }
}