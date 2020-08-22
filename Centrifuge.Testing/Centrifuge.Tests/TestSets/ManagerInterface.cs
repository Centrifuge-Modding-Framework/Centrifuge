using Centrifuge.TestFramework;

namespace Centrifuge.Tests.TestSets
{
    [TestSet]
    public class ManagerInterface
    {
        [Test]
        public void ManagerIsNotNull()
            => Assert.IsNotNull(Mod.Manager);

        [Test]
        public void HotkeyManagerIsNotNull()
            => Assert.IsNotNull(Mod.Manager.Hotkeys);

        [Test]
        public void WasInitFinishedCalled()
            => Assert.True(Mod.InitFinishedCalled);

        [Test]
        public void LoadedModCountIsNotZero()
            => Assert.NotZero(Mod.Manager.GetLoadedMods().Count);
        
        [Test]
        public void GetModReturnsCorrectInfo()
        {
            var thisMod = Mod.Manager.GetMod(Mod.ModID);

            Assert.AreEqual(Mod.ModID, thisMod.ModID);
            Assert.AreSame(thisMod, Mod.Instance);
        }
    }
}