import React from "react";
import NavMenu from "./NavMenu";

// Router
import {
	BrowserRouter as Router,
	Switch,
	Route,
	Redirect,
} from "react-router-dom";

import InfoView from "./Views/InfoView/Info";
import Debug from "./Views/DebugView/Debug";
import Setting from "./Views/SettingsView/Setting";
import Villages from "./Views/VillagesView/Villages";
import Hero from "./Views/HeroView/Hero";
import NewVillages from "./Views/NewVillageView/NewVilalge";
import Overview from "./Views/Overview/Overview";
import FarmingView from "./Views/FarmingView/FarmingView";

const Layout = () => {
	return (
		<Router>
			<NavMenu />
			<div style={{ margin: "1%" }}>
				<Switch>
					<Route path={"/setting"}>
						<Setting />
					</Route>
					<Route path={"/hero"}>
						<Hero />
					</Route>
					<Route path={"/villages"}>
						<Villages />
					</Route>
					<Route path={"/newvills"}>
						<NewVillages />
					</Route>
					<Route path={"/overview"}>
						<Overview />
					</Route>
					<Route path={"/debug"}>
						<Debug />
					</Route>
					<Route path={"/farming"}>
						<FarmingView />
					</Route>
					<Route path={"/info"}>
						<InfoView />
					</Route>

					<Route path="*">
						<Redirect to="/info" />
					</Route>
				</Switch>
			</div>
		</Router>
	);
};

export default Layout;