import React, { useState, useEffect } from "react";
import { Tabs, Tab } from "@mui/material";
import {
	Link,
	useRouteMatch,
	Switch,
	Route,
	Redirect,
	useLocation,
} from "react-router-dom";
import NormalBuilding from "./NormalBuilding";
import PrerequisiteBuilding from "./PrerequisiteBuilding";
import ResourceBuilding from "./ResourceBuilding";

const ButtonBuildingModal = () => {
	const location = useLocation();
	const { path, url } = useRouteMatch();
	const [value, setValue] = useState("/normal");

	const handleChange = (event, newValue) => {
		setValue(newValue);
	};

	useEffect(() => {
		const path = location.pathname.split("/")[3];
		console.log(path);
		setValue(`/${path}`);
	}, [location]);
	return (
		<>
			<Tabs value={value} onChange={handleChange} textColor="inherit">
				<Tab
					label="Normal"
					value="/normal"
					to={`${url}/normal`}
					component={Link}
				/>
				<Tab
					label="Resource"
					value="/resource"
					to={`${url}/resource`}
					component={Link}
				/>
				<Tab
					label="Prerequisite"
					value="/prerequisite"
					to={`${url}/prerequisite`}
					component={Link}
				/>
			</Tabs>
			<div style={{ margin: "1%" }}>
				<Switch>
					<Route exact path={path}>
						<Redirect to={`${path}/normal`} />
					</Route>
					<Route path={`${path}/normal`}>
						<NormalBuilding />
					</Route>
					<Route path={`${path}/resource`}>
						<ResourceBuilding />
					</Route>
					<Route path={`${path}/prerequisite`}>
						<PrerequisiteBuilding />
					</Route>
				</Switch>
			</div>
		</>
	);
};

export default ButtonBuildingModal;
