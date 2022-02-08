import React, { useEffect, useState } from "react";
import { Typography, TextareaAutosize } from "@mui/material";

import { getLogData } from "../../../api/Debug";
import { signalRConnection } from "../../../realtime/connection";

import { useSelector } from "react-redux";
import { HubConnectionState } from "@microsoft/signalr/dist/esm/HubConnection";
const LogBoard = () => {
	const [value, setValue] = useState([]);
	const account = useSelector((state) => state.account.info.id);
	const signalr = useSelector((state) => state.signalr);

	useEffect(() => {
		if (signalRConnection.State === HubConnectionState.Connected) {
			signalRConnection.on("logger", (data) => {
				setValue((prev) => `${data}\n${prev}`);
			});

			return () => {
				signalRConnection.off("logger");
			};
		}
	}, [signalr]);

	useEffect(() => {
		if (account !== -1) {
			getLogData(account).then((data) => setValue(data));
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
				value={value.join("\n")}
				readOnly={true}
				defaultValue="Nothing here"
			/>
		</>
	);
};
export default LogBoard;
