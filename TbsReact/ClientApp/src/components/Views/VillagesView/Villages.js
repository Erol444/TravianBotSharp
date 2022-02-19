import React, { useState } from "react";

import { Grid, Tabs, Tab, Box } from "@mui/material";
import { Link, useRouteMatch, Switch, Route, Redirect } from "react-router-dom";

import style from "../../../styles/box";
import VillagesTable from "./VillagesTable";
import Build from "./BuildView/Build";
import Market from "./SubView/Market";
import Troops from "./SubView/Troops";
import Attack from "./SubView/Attack";
import Farming from "./SubView/Farming";
import Info from "./SubView/Info";

const Villages = () => {
	const { path, url } = useRouteMatch();
	const [value, setValue] = useState("/build");

	const handleChange = (event, newValue) => {
		setValue(newValue);
	};
	return (
		<>
			<Grid
				container
				spacing={3}
				direction="row"
				justifyContent="flex-start"
				alignItems="baseline"
			>
				<Grid item xs={3}>
					<VillagesTable />
				</Grid>
				<Grid item xs={9}>
					<Box sx={style}>
						<Tabs
							value={value}
							onChange={handleChange}
							textColor="inherit"
						>
							<Tab
								label="Build"
								value="/build"
								to={`${url}/build`}
								component={Link}
							/>
							<Tab
								label="Market"
								value="/market"
								to={`${url}/market`}
								component={Link}
							/>
							<Tab
								label="Troops"
								value="/troops"
								to={`${url}/troops`}
								component={Link}
							/>
							<Tab
								label="Attack"
								value="/attack"
								to={`${url}/attack`}
								component={Link}
							/>
							<Tab
								label="Farming"
								value="/farming"
								to={`${url}/farming`}
								component={Link}
							/>
							<Tab
								label="Info"
								value="/info"
								to={`${url}/info`}
								component={Link}
							/>
						</Tabs>
						<div style={{ margin: "1%" }}>
							<Switch>
								<Route exact path={path}>
									<Redirect to={`${path}/build`} />
								</Route>
								<Route path={`${path}/build`}>
									<Build />
								</Route>
								<Route path={`${path}/market`}>
									<Market />
								</Route>
								<Route path={`${path}/troops`}>
									<Troops />
								</Route>
								<Route path={`${path}/attack`}>
									<Attack />
								</Route>
								<Route path={`${path}/farming`}>
									<Farming />
								</Route>
								<Route path={`${path}/info`}>
									<Info />
								</Route>
							</Switch>
						</div>
					</Box>
				</Grid>
			</Grid>
		</>
	);
};

export default Villages;
