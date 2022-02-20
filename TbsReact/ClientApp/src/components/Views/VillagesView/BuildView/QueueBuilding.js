import { Typography } from "@mui/material";
import React, { useState, useEffect } from "react";
import ContentBox from "../../../ContentBox";
import MUIDraggableTable from "../../../ref/MUIDraggableTable";
import { arrayMoveImmutable } from "array-move";

const QueueBuilding = () => {
	const header = ["Building", "Level", "Location"];
	const data = [
		{ id: 0, building: "Loading ...", level: 3, location: 23 },
		{ id: 1, building: "Loading ...", level: 2, location: 44 },
		{ id: 2, building: "Loading ...", level: 1, location: 55 },
		{ id: 3, building: "Loading ...", level: 0, location: 66 },
		{ id: 4, building: "Loading ...", level: 0, location: 77 },
	];

	const [items, setItems] = useState(data);

	const onRemove = (index) => {
		setItems((items) => items.filter((item) => item.id !== index));
		console.log("remove", index);
	};

	const onSortEnd = ({ oldIndex, newIndex }) => {
		setItems(arrayMoveImmutable(items, oldIndex, newIndex));
		console.log("sort", oldIndex, newIndex);
	};

	useEffect(() => {
		console.log(items);
	}, [items]);

	return (
		<>
			<ContentBox>
				<Typography variant="h5">Queue Building</Typography>
				<MUIDraggableTable
					header={header}
					data={items}
					onRemove={onRemove}
					onSortEnd={onSortEnd}
					useDragHandle
				/>
			</ContentBox>
		</>
	);
};

export default QueueBuilding;
