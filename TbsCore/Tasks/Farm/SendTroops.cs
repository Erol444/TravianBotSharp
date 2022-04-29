using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.SendTroopsModels;
using TbsCore.Models.TroopsModels;
using TbsCore.Parsers;

namespace TbsCore.Tasks.LowLevel
{
    public class SendTroops : BotTask
    {
        public TroopsSendModel TroopsMovement { get; set; }

        /// <summary>
        /// Other tasks (like SendDeff) can extend this task and configure amount of troops to
        /// send when getting amount of troops at home. If false is returned, bot won't proceed the attack
        /// </summary>
        public Func<Account, TroopsBase, bool> TroopsCallback { get; set; }

        /// <summary>
        /// When the troops will arrive to the destination, is set after the bot sends the troops.
        /// Used by other BotTasks that extend this task (like OasisFarming)
        /// </summary>
        public TimeSpan Arrival { get; private set; }

        /// <summary>
        /// In case we don't have specified troops at home, send partial attack (as many troops as we have at home)
        /// If false, bot will skip this task.
        /// </summary>
        public bool SendPartialAttack { get; set; } = false;

        /// <summary>
        /// Whether we want to embed coordinates into the url. This saves ~1 sec and it used when searching from the map / send troops clicked
        /// </summary>
        //public bool SetCoordsInUrl { get; set; } = false; // Currently it will navigate to coordinates only on TTwars

        public override async Task<TaskRes> Execute(Account acc)
        {
            //if (acc.AccInfo.ServerVersion == Classificator.ServerVersionEnum.TTwars) SetCoordsInUrl = true;
            {
                acc.Logger.Information($"Checking current village ...");
                var result = await NavigationHelper.SwitchVillage(acc, Vill);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }

            await NavigationHelper.ToRallyPoint(acc, Vill,
                NavigationHelper.RallyPointTab.SendTroops, TroopsMovement.TargetCoordinates
                //SetCoordsInUrl ? TroopsMovement.TargetCoordinates : null
                );

            var troopsArr = TroopsMovementParser.GetTroopsInRallyPoint(acc.Wb.Html);
            var proceed = TroopsCallback?.Invoke(acc, new TroopsBase(troopsArr, acc.AccInfo.Tribe));
            if (!(proceed ?? true)) return TaskRes.Retry;

            // No troops selected to be sent from this village
            if (this.TroopsMovement.Troops.Sum() == 0) return TaskRes.Executed;

            // Add number of troops to the input boxes
            for (int i = 0; i < TroopsMovement.Troops.Length; i++)
            {
                if (TroopsMovement.Troops[i] == 0) continue;
                if (troopsArr[i] < TroopsMovement.Troops[i])
                {
                    if (SendPartialAttack)
                    {
                        TroopsMovement.Troops[i] = troopsArr[i];
                    }
                    else
                    {
                        acc.Logger.Warning($"Vill {this.Vill} didn't have enough troops to send! Skipping task");
                        return TaskRes.Executed;
                    }
                }

                switch (acc.AccInfo.ServerVersion)
                {
                    case Classificator.ServerVersionEnum.TTwars:
                        await DriverHelper.WriteByName(acc, $"t{i + 1}", TroopsMovement.Troops[i]);
                        break;

                    case Classificator.ServerVersionEnum.T4_5:
                        await DriverHelper.WriteByName(acc, $"troops[0][t{i + 1}]", TroopsMovement.Troops[i]);
                        break;
                }
            }

            // Select coordinates, if we haven't set them in the url already
            await DriverHelper.WriteCoordinates(acc, TroopsMovement.TargetCoordinates);

            //Select type of troop sending
            string script = $"Array.from(document.getElementsByName('c')).find(x=>x.value=={(int)TroopsMovement.MovementType}).checked=true;";
            await DriverHelper.ExecuteScript(acc, script);

            //Click on "Send" button
            await DriverHelper.ClickById(acc, "btn_ok");

            await Task.Delay(AccountHelper.Delay(acc));

            // Select catapult targets
            if (this.TroopsMovement.Target1 != Classificator.BuildingEnum.Site)
                await DriverHelper.SelectIndexByName(acc, "ctar1", (int)this.TroopsMovement.Target1);
            if (this.TroopsMovement.Target2 != Classificator.BuildingEnum.Site)
                await DriverHelper.SelectIndexByName(acc, "ctar2", (int)this.TroopsMovement.Target2);

            // Scout type
            if (this.TroopsMovement.ScoutType != ScoutEnum.None)
            {
                string scout = $"Array.from(document.getElementsByName('spy')).find(x=>x.value=={(int)TroopsMovement.ScoutType}).checked=true;";
                await DriverHelper.ExecuteScript(acc, scout);
            }

            // Parse movement duration of the troops
            this.Arrival = TroopsMovementParser.GetMovementDuration(acc.Wb.Html);

            //Click on "Send" button
            await DriverHelper.ClickById(acc, "btn_ok");
            acc.Logger.Information($"Bot sent troops from village {Vill.Name} to {this.TroopsMovement.TargetCoordinates}");

            return TaskRes.Executed;
        }
    }
}