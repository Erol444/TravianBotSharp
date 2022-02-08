import React from "react";
import NavMenu from "./NavMenu";

// Router
import {
	BrowserRouter as Router,
	Switch,
	Route,
	Redirect,
} from "react-router-dom";

// info view
import Info from "./Views/Info";

// debug view
import Debug from "./Views/DebugView/Debug";

import Setting from "./Views/SettingsView/Setting";

import Villages from "./Views/VillagesView/Villages";
import Hero from "./Views/HeroView/Hero";
import NewVillages from "./Views/NewVillageView/NewVilalge";

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
					<Route path={"/debug"}>
						<Debug />
					</Route>
					<Route path={"/info"}>
						<Info />
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
