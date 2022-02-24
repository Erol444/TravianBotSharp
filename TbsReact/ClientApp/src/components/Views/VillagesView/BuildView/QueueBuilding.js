import { Typography } from "@mui/material";
import React, { useState, useEffect } from "react";
import ContentBox from "../../../ContentBox";
import MUIDraggableTable from "../../../ref/MUIDraggableTable";
import { arrayMoveImmutable } from "array-move";
import { useSelector } from "react-redux";
import { useVillage } from "../../../../hooks/useVillage";
import { getQueueList, editQueue, deleteQueue } from "../../../../api/api";

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
	const account = useSelector((state) => state.account.info);
	const [villageId] = useVillage();
	useEffect(() => {
		if (account.id !== -1 && villageId !== -1) {
			getQueueList(account.id, villageId).then((data) => {
				setItems(data);
			});
		}
	}, [account.id, villageId]);

	const onRemove = (index) => {
		setItems((items) => items.filter((item) => item.id !== index));
		deleteQueue(account.id, villageId, index);
	};

	const onSortEnd = ({ oldIndex, newIndex }) => {
		setItems(arrayMoveImmutable(items, oldIndex, newIndex));
		editQueue(account.id, villageId, {
			indexOld: oldIndex,
			indexNew: newIndex,
		});
	};

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
