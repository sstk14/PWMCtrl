using System;
using System.Collections.Generic;
using System.Text;

namespace PWMCtrl
{
    public class PWMCtrl
    {

        public const int PWM_MODE_MARK_SPACE = 0;


        public const int PWM_OUTPUT = 2;

        public const int PWM_RANGE = 1024;

        
        /// <summary>
        /// -90°指定の時のマイクロ秒
        /// </summary>
        public const int DEG_M90_MicroSEC = 600;

        /// <summary>
        /// +90°指定の時のマイクロ秒
        /// </summary>
        public const int DEG_P90_MicroSEC = 2300;

        
        /// <summary>
        /// モーターの動作周波数
        /// </summary>
        public int HzOfMorter = 50;


        /// <summary>
        /// モーターのパルス周期
        /// </summary>
        public int PulsCycleOfMorter = 20;


        /// <summary>
        /// PWMクロック数を求める
        /// </summary>
        /// <returns></returns>
        public int GetPWMClock()
        {
            //PWMクロック  = ラズパイの動作周波数(19.2MHz) / (サーボモーター周波数 * PWMレンジ)
            //PWMレンジは、Wiring Piのデフォルト値=1024を指定
            var pwmClock = 19200000 / (HzOfMorter * PWM_RANGE);
            return pwmClock;
        }

        /// <summary>
        /// デューティー比を取得する
        /// </summary>
        /// <returns></returns>
        public double GetDutyCycle(int deg)
        {
            double dutyCycle = 0.0;
            try
            {
                //電圧をかける時間(ms) : 20 = x : PWMレンジ
                double msecTypApplyVol = this.GetMsecToApplyVoltage(deg);
                dutyCycle = (PWM_RANGE * msecTypApplyVol) / PulsCycleOfMorter;
                return dutyCycle;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 指定した角度に動かすために電圧をかける時間を取得します
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public double GetMsecToApplyVoltage(int deg)
        {
            if (deg > 90 || -90 > deg)
            {
                throw new ArgumentException("引数が不正です。入力範囲[-90～+90]");
            }

            deg = deg - (-90);
            //1°動くのに必要なマイクロ秒
            double tmpVal = (PWMCtrl.DEG_P90_MicroSEC - PWMCtrl.DEG_M90_MicroSEC) / 180.0;
            tmpVal = (tmpVal  * deg) + PWMCtrl.DEG_M90_MicroSEC;
            double msecToApplyVol = tmpVal / 1000.0;
            return Math.Round(msecToApplyVol, 1, MidpointRounding.AwayFromZero);
        }


    }
}
