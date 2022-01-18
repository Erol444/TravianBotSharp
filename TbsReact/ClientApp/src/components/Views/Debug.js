import React from "react";
import Grid from "@mui/material/Grid";

import LogBoard from "./DebugChild/LogBoard";
import TaskTable from "./DebugChild/TaskTable";
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
