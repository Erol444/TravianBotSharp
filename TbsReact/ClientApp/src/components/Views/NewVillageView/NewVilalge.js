import React from "react";
import { Grid } from "@mui/material";
import ContentBox from "../../ContentBox";

const NewVillages = () => {
	return (
		<ContentBox>
			<Grid container spacing={2}>
				<Grid item xs={4}>
					<Grid container spacing={2}>
						<Grid item xs={12}>
							<ContentBox name="Village template name" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Auto find checkbox" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Village type will find" />
						</Grid>
					</Grid>
				</Grid>
				<Grid item xs={4}>
					<Grid container spacing={2}>
						<Grid item xs={12}>
							<ContentBox name="New village list" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Add village manual" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Button for auto add" />
						</Grid>
					</Grid>
				</Grid>
			</Grid>
		</ContentBox>
	);
};

export default NewVillages;
