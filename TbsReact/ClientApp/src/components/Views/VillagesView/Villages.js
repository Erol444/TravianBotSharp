import React, { useState } from "react";

import { Grid, Tabs, Tab, Box } from "@mui/material";
import { Link, useRouteMatch, Switch, Route, Redirect } from "react-router-dom";

import style from "../../../styles/box";
import VillagesTable from "./VillagesTable";
import ContentBox from "../../ContentBox";

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
				<Grid item xs={2}>
					<VillagesTable />
				</Grid>
				<Grid item xs={8}>
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
									<ContentBox name="build" />
								</Route>
								<Route path={`${path}/market`}>
									<ContentBox name="market" />
								</Route>
								<Route path={`${path}/troops`}>
									<ContentBox name="troops" />
								</Route>
								<Route path={`${path}/attack`}>
									<ContentBox name="attack" />
								</Route>
								<Route path={`${path}/farming`}>
									<ContentBox name="farming" />
								</Route>
								<Route path={`${path}/info`}>
									<ContentBox name="info" />
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
