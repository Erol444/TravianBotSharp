import { Typography } from "@mui/material";
import React, { useState } from "react";
import ContentBox from "../../../ContentBox";
import MUITable from "../../../ref/MUITable";

const QueueBuilding = () => {
	const header = ["Building", "Level", "Location"];
	const data = [
		{ id: 0, building: "Loading ...", level: 3, location: 23 },
		{ id: 1, building: "Loading ...", level: 2, location: 44 },
	];
	const [selected, setSelected] = useState(0);

	const onClick = (vill) => {
		setSelected(vill.id);
	};
	return (
		<>
			<ContentBox>
				<Typography variant="h5">Queue Building</Typography>
				<MUITable
					header={header}
					data={data}
					handler={onClick}
					selected={selected}
				/>
			</ContentBox>
		</>
	);
};

export default QueueBuilding;
