import React from "react";
import Grid from "@mui/material/Grid";

import LogBoard from "./LogBoard";
import TaskTable from "./TaskTable";
const Debug = () => {
	return (
		<>
			<Grid container spacing={2}>
				<Grid item xs={6}>
					<TaskTable />
				</Grid>
				<Grid item xs={6}>
					<LogBoard />
				</Grid>
			</Grid>
		</>
	);
};

export default Debug;
