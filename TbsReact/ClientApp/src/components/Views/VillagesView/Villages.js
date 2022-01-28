import React from "react";
import { Grid } from "@mui/material";
import ContentBox from "./ContentBox";
import VillagesTable from "./VillagesTable";

const Villages = () => {
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
					<ContentBox />
				</Grid>
			</Grid>
		</>
	);
};

export default Villages;
