import { Typography } from "@mui/material";
import React, { useState } from "react";
import ContentBox from "../../../ContentBox";
import MUITable from "../../../ref/MUITable";

const CurrentBuilding = () => {
	const header = ["Building", "Level", "Time"];
	const data = [
		{
			id: 0,
			building: "Loading ...",
			level: 3,
			time: "2022/02/02 02:02:02",
		},
		{
			id: 1,
			building: "Loading ...",
			level: 2,
			time: "2022/02/02 02:02:02",
		},
	];
	const [selected, setSelected] = useState(0);

	const onClick = (vill) => {
		setSelected(vill.id);
	};
	return (
		<>
			<ContentBox>
				<Typography variant="h5">Current Building</Typography>
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

export default CurrentBuilding;
