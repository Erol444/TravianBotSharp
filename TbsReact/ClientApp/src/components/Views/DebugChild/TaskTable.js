import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";

import { getTaskList } from "../../../api/Debug";
import { signalRConnection } from "../../../realtime/connection";
import { DataGrid } from "@mui/x-data-grid";
import { Typography } from "@mui/material";

const TaskTable = ({ selected, isConnect }) => {
	const [value, setValue] = useState([
		{
			id: 0,
			name: "hi",
			villName: "My",
			priority: "name",
			stage: "is",
			executeAt: "TbsReact",
		},
	]);
	const columns = [
		{ field: "name", headerName: "Name" },
		{ field: "villName", headerName: "Village", width: 100 },
		{ field: "priority", headerName: "Priority" },
		{ field: "stage", headerName: "Stage" },
		{ field: "executeAt", headerName: "Execute at", width: 100 },
	];

	useEffect(() => {
		if (isConnect === true) {
			signalRConnection.on("task", (message) => {
				console.log("task message", message);
				if (message === "waiting") {
					setValue([
						{
							id: 0,
							name: "hi",
							villName: "Bot",
							priority: "is",
							stage: "loading",
							executeAt: "task",
						},
					]);
				} else if (message === "reset") {
					if (selected !== -1) {
						const getData = async () => {
							const data = await getTaskList(selected);
							if (data === null) {
								setValue([
									{
										id: 0,
										name: "hi",
										villName: "Account",
										priority: "has",
										stage: "no",
										executeAt: "task",
									},
								]);
							} else {
								setValue(data);
							}
						};
						getData();
					}
				}
			});

			return () => {
				signalRConnection.off("task");
			};
		}
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [isConnect]);

	useEffect(() => {
		if (selected !== -1) {
			const getData = async () => {
				const data = await getTaskList(selected);
				if (data === null) {
					setValue([
						{
							id: 0,
							name: "hi",
							villName: "Account",
							priority: "has",
							stage: "no",
							executeAt: "task",
						},
					]);
				} else {
					setValue(data);
				}
				console.log({ data });
			};
			getData();
		}
	}, [selected]);
	return (
		<>
			<Typography variant="h6" noWrap>
				Task
			</Typography>
			<div style={{ height: 400, width: "100%" }}>
				<div style={{ height: "100%", display: "flex" }}>
					<div style={{ flexGrow: 1 }}>
						<DataGrid
							rows={value}
							columns={columns}
							density="compact"
							disableSelectionOnClick={true}
							hideFooterSelectedRowCount={true}
						/>
					</div>
				</div>
			</div>
		</>
	);
};
TaskTable.propTypes = {
	selected: PropTypes.number.isRequired,
	isConnect: PropTypes.bool.isRequired,
};
export default TaskTable;
