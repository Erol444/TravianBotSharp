import React from "react";
import { Grid } from "@mui/material";
import ContentBox from "../../../ContentBox";
import VillageBuilding from "./VillageBuilding";
import CurrentBuilding from "./CurrentBuilding";
import QueueBuilding from "./QueueBuilding";
import ButtonBuilding from "./ButtonBuilding";
import OptionBuilding from "./OptionBuilding";

const Build = () => {
	return (
		<ContentBox>
			<Grid container spacing={2}>
				<Grid item xs={4}>
					<VillageBuilding />
				</Grid>
				<Grid item xs={8}>
					<Grid container spacing={2}>
						<Grid item xs={3}>
							<ContentBox>
								<ButtonBuilding />
								<br />
								<br />
								<OptionBuilding />
							</ContentBox>
						</Grid>
						<Grid item xs={9}>
							<CurrentBuilding />
						</Grid>

						<Grid item xs={12}>
							<QueueBuilding />
						</Grid>
					</Grid>
				</Grid>
			</Grid>
		</ContentBox>
	);
};

export default Build;
