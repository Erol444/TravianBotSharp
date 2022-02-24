import React, { useEffect, useState } from "react";
import { Typography, TextareaAutosize } from "@mui/material";

import { getLogData } from "../../../api/Debug";

import { useSelector } from "react-redux";
import { HubConnectionState } from "@microsoft/signalr/dist/esm/HubConnection";
import { useSignalR } from "../../../hooks/useSignalR";

const LogBoard = () => {
	const [value, setValue] = useState("Loading ...");
	const account = useSelector((state) => state.account.info.id);
	const signalRConnection = useSignalR();
	useEffect(() => {
		if (signalRConnection.State === HubConnectionState.Connected) {
			signalRConnection.on("logger", (data) => {
				console.log(data);
				setValue((prev) => `${data}\n${prev}`);
			});

			return () => {
				signalRConnection.off("logger");
			};
		}
	}, [signalRConnection]);

	useEffect(() => {
		if (account !== -1) {
			getLogData(account).then((data) => {
				if (data.length > 0) {
					setValue(data);
				} else {
					setValue("No Logs");
				}
			});
		}
	}, [account]);
	return (
		<>
			<Typography variant="h6" noWrap>
				Log
			</Typography>
			<TextareaAutosize
				style={{ width: "100%" }}
				maxRows={20}
				minRows={20}
				value={value}
				readOnly={true}
			/>
		</>
	);
};
export default LogBoard;
