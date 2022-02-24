import React, { useEffect, useState } from "react";

import { getTaskList } from "../../../api/Debug";
import { DataGrid } from "@mui/x-data-grid";
import { Typography } from "@mui/material";
import { useSelector } from "react-redux";
import { HubConnectionState } from "@microsoft/signalr/dist/esm/HubConnection";
import { useSignalR } from "../../../hooks/useSignalR";

const TaskTable = () => {
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

	const account = useSelector((state) => state.account.info.id);
	const signalRConnection = useSignalR();
	useEffect(() => {
		if (signalRConnection.State === HubConnectionState.Connected) {
			signalRConnection.on("task", (message) => {
				console.log(message);
				switch (message) {
					case "waiting":
						setValue([
							{
								id: 0,
								name: "hi",
								villName: "TBS",
								priority: "is",
								stage: "loading",
								executeAt: "task",
							},
						]);
						break;

					case "reset":
						if (account === -1) break;
						getTaskList(account).then((data) => {
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
						});

						break;
				}
			});

			return () => {
				signalRConnection.off("task");
			};
		}
	}, [account, signalRConnection]);

	useEffect(() => {
		if (account !== -1) {
			getTaskList(account).then((data) => {
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
			});
		}
	}, [account]);
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
export default TaskTable;
