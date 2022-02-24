import React, { useState, createContext } from "react";
import { Grid } from "@mui/material";
import ContentBox from "../../../ContentBox";
import VillageBuilding from "./VillageBuilding";
import CurrentBuilding from "./CurrentBuilding";
import QueueBuilding from "./QueueBuilding";
import ButtonBuilding from "./ButtonBuilding";
import OptionBuilding from "./OptionBuilding";

const BuildingContext = createContext();

const Build = () => {
	const [selected, setSelected] = useState(-1);
	const buildingValue = [selected, setSelected];
	return (
		<BuildingContext.Provider value={buildingValue}>
			<ContentBox>
				<Grid container spacing={2}>
					<Grid item xs={4}>
						<VillageBuilding />
					</Grid>
					<Grid item xs={8}>
						<Grid container spacing={2}>
							<Grid item xs={3}>
								<ContentBox>
									<center>
										<ButtonBuilding />
										<br />
										<br />
										<OptionBuilding />
									</center>
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
		</BuildingContext.Provider>
	);
};

export { BuildingContext };
export default Build;
