using System;
using System.Runtime.InteropServices;
using PWMCtrl;

namespace PWMCtrl
{
    class Program
    {
        [DllImport("wiringPi")]
        extern static int wiringPiSetupGpio();
        [DllImport("wiringPi")]
        extern static void pinMode(int pin, int mode);

        [DllImport("wiringPi")]
        extern static void pwmSetMode(int mode);

        [DllImport("wiringPi")]
        extern static void pwmSetRange(uint range);

        [DllImport("wiringPi")]
        extern static void pwmSetClock(int divisor);

        [DllImport("wiringPi")]
        extern static void pwmWrite(int pin, int value);


        static void Main(string[] args)
        {
            try
            {
                int deg = 0;
                if(int.TryParse(args[0], out deg) == false)
                {
                    Console.WriteLine(string.Format("引数'{0}'が不正です。数字を入力してください。", args[0]));
                    return;
                }
                if(deg > 90 || -90 > deg)
                {

                    Console.WriteLine(string.Format("引数が不正です。入力範囲[-90～+90]"));
                    return;
                }
                PWMCtrl pwmCtrl = new PWMCtrl();
                int duty = (int)pwmCtrl.GetDutyCycle(deg);
                double tmp = pwmCtrl.GetMsecToApplyVoltage(deg);

                //wiringPiのセットアップ
                wiringPiSetupGpio();

                //GPIO18をPWM_OUTPUT(2)に設定する
                pinMode(18, PWMCtrl.PWM_OUTPUT);

                //PWMモードをmark-spaceに設定する
                pwmSetMode(PWMCtrl.PWM_MODE_MARK_SPACE);

                //PWMのレンジを1024にする
                pwmSetRange(PWMCtrl.PWM_RANGE);

                //PWMクロックを375kHzにする
                var pwmClock = pwmCtrl.GetPWMClock();
                pwmSetClock(pwmClock);

                //*** モーターを回転させる ***

                Console.WriteLine(string.Format("{0}ms, {1}duty", tmp, duty));
                Console.WriteLine(duty);
                pwmWrite(18, duty); ///指定した角度に指定する

                //0°に戻す
                System.Threading.Thread.Sleep(1000);
                //duty = (int)pwmCtrl.GetDutyCycle(0);
                //tmp = pwmCtrl.GetMsecToApplyVoltage(0);
                //Console.WriteLine(string.Format("{0}ms, {1}duty", tmp, duty));
                //pwmWrite(18, duty);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
