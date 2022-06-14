using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TbsCore.Helpers;
using TbsCore.Models.MapModels;
using TbsCore.Models.VillageModels;
using TbsCore.Tasks.Others;
using TbsCore.Tasks.Sim;
using TbsWinformNet6.Helpers;
using TbsWinformNet6.Interfaces;
using TbsWinformNet6.Views.BaseViews;
using Keys = OpenQA.Selenium.Keys;

namespace TbsWinformNet6.Views
{
    public partial class NewVillagesUc : TbsBaseUc, ITbsUc
    {
        public NewVillagesUc()
        {
            InitializeComponent();
        }

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            if (acc == null) return;

            BuildTasksLocationTextBox.Text = acc.NewVillages.BuildingTasksLocationNewVillage;

            //new villages coords list
            NewVillList.Items.Clear();
            if (acc.NewVillages.Locations.Count > 0)
            {
                foreach (var newvill in acc.NewVillages.Locations)
                {
                    var item = new ListViewItem();
                    item.SubItems[0].Text = newvill.Coordinates.x.ToString();
                    item.SubItems.Add(newvill.Coordinates.y.ToString());
                    item.SubItems.Add(newvill.Name);
                    NewVillList.Items.Add(item);
                }
            }

            //new village types to find
            villTypeView.Items.Clear();
            if (acc.NewVillages.Types.Count > 0)
            {
                foreach (var type in acc.NewVillages.Types)
                {
                    villTypeView.Items.Add(new ListViewItem(type.ToString().Replace("_", "")));
                }
            }

            valleyType.SelectedIndex = 0;
            checkBox3.Checked = acc.NewVillages.AutoSettleNewVillages;
            autoNewVillagesToSettle.Checked = acc.NewVillages.AutoFindVillages;
            villName.Text = acc.NewVillages.NameTemplate;
        }

        private void removeNewVill_Click(object sender, EventArgs e)
        {
            var indicies = NewVillList.SelectedIndices;
            if (indicies.Count > 0)
            {
                var i = indicies[0];
                NewVillList.Items.RemoveAt(i);
                GetSelectedAcc().NewVillages.Locations.RemoveAt(i);
            }
        }

        private void confirmNewVill_Click(object sender, EventArgs e)
        {
            Coordinates c = new Coordinates();
            c.x = (int)XNewVill.Value;
            c.y = (int)YNewVill.Value;
            GetSelectedAcc().NewVillages.Locations.Add(new NewVillage() { Coordinates = c, Name = NewVillName.Text });
            //clear values
            XNewVill.Value = 0;
            YNewVill.Value = 0;
            NewVillName.Text = "";
            UpdateUc();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            var selected = (string)valleyType.SelectedItem;
            selected = "_" + selected;
            var type = (Classificator.VillTypeEnum)Enum.Parse(typeof(Classificator.VillTypeEnum), selected);
            acc.NewVillages.Types.Add(type);
            UpdateUc();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetSelectedAcc().NewVillages.Types.Clear();
            UpdateUc();
        }

        private void autoNewVillagesToSettle_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().NewVillages.AutoFindVillages = autoNewVillagesToSettle.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GetSelectedAcc().NewVillages.Locations.Clear();
            UpdateUc();
        }

        private void villName_TextChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().NewVillages.NameTemplate = villName.Text;
        }

        private void Button25_Click(object sender, EventArgs e) //select building tasks for new village
        {
            string location = IoHelperForms.PromptUserForBuidTasksLocation();
            GetSelectedAcc().NewVillages.BuildingTasksLocationNewVillage = location;
            UpdateUc();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().NewVillages.AutoSettleNewVillages = checkBox3.Checked;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            GetSelectedAcc().Tasks.Add(new FindVillageToSettle() { ExecuteAt = DateTime.Now });
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            var task = new HeroSetPoints();
            try
            {
                await task.Execute(acc);
            }
            catch (Exception ex)
            {
                _ = ex;
            }
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            var items = new List<(Classificator.HeroItemEnum, int)>
            {
                new (Classificator.HeroItemEnum.Resource_Crop_0, 1),
            };
            var task = new HeroEquip() { Items = items };
            try
            {
                await task.Execute(acc);
            }
            catch (Exception ex)
            {
                _ = ex;
            }
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            try
            {
                var acc = GetSelectedAcc();
                acc.Wb.UpdateHtml();
                var item = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("item148"));
                item = item.ParentNode;
                var elements = acc.Wb.Driver.FindElements(By.XPath(item.XPath));
                elements[0].Click();
                var wait = new WebDriverWait(acc.Wb.Driver, TimeSpan.FromMinutes(1));
                wait.Until(driver => driver.FindElements(By.Id("consumableHeroItem")).Count > 0);
                acc.Wb.UpdateHtml();

                var form = acc.Wb.Html.GetElementbyId("consumableHeroItem");
                var input = form.Descendants("input").FirstOrDefault();
                var inputElements = acc.Wb.Driver.FindElements(By.XPath(input.XPath));

                inputElements[0].SendKeys(Keys.Home);
                inputElements[0].SendKeys(Keys.Shift + Keys.End);
                inputElements[0].SendKeys($"{1}");

                var dialog = acc.Wb.Html.GetElementbyId("dialogContent");
                var buttonWrapper = dialog.Descendants("div").FirstOrDefault(x => x.HasClass("buttonsWrapper"));
                var buttonTransfer = buttonWrapper.Descendants("button").ToArray();
                var buttonTransferElements = acc.Wb.Driver.FindElements(By.XPath(buttonTransfer[1].XPath));
                buttonTransferElements[0].Click();

                wait.Until(driver =>
                {
                    acc.Wb.UpdateHtml();
                    var inventoryPageWrapper = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("inventoryPageWrapper"));
                    return !inventoryPageWrapper.HasClass("loading");
                });

                _ = dialog;
            }
            catch (Exception ex)
            {
                _ = ex;
            }
        }
    }
}