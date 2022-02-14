import React, { useState } from "react";
import { Tabs, Tab } from "@mui/material";
import { Link, useRouteMatch, Switch, Route, Redirect } from "react-router-dom";
import ContentBox from "../../ContentBox";

import ChromeSettings from "./ChromeSettings";
import ActivitySettings from "./ActivitySettings";
import QuestSettings from "./QuestSettings";
import DiscordSettings from "./DiscordSettings";

const General = () => {
	const { path, url } = useRouteMatch();
	const [value, setValue] = useState("/chrome");

	const handleChange = (event, newValue) => {
		setValue(newValue);
	};
	return (
		<>
			<ContentBox>
				<Tabs value={value} onChange={handleChange} textColor="inherit">
					<Tab
						label="Chome"
						value="/chrome"
						to={`${url}/chrome`}
						component={Link}
					/>
					<Tab
						label="Activity"
						value="/activity"
						to={`${url}/activity`}
						component={Link}
					/>
					<Tab
						label="Quest"
						value="/Quest"
						to={`${url}/quest`}
						component={Link}
					/>
					<Tab
						label="Discord"
						value="/discord"
						to={`${url}/discord`}
						component={Link}
					/>
				</Tabs>
				<div style={{ margin: "1%" }}>
					<Switch>
						<Route exact path={path}>
							<Redirect to={`${path}/chrome`} />
						</Route>
						<Route path={`${path}/chrome`}>
							<ChromeSettings />
						</Route>
						<Route path={`${path}/activity`}>
							<ActivitySettings />
						</Route>
						<Route path={`${path}/quest`}>
							<QuestSettings />
						</Route>
						<Route path={`${path}/discord`}>
							<DiscordSettings />
						</Route>
					</Switch>
				</div>
			</ContentBox>
		</>
	);
};

export default General;
