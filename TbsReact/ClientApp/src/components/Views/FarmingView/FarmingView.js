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
							<ContentBox name="Interval setting" />
						</Grid>
						<Grid item xs={6}>
							<ContentBox name="Start farm manually buttons" />
						</Grid>
						<Grid item xs={6}>
							<ContentBox name="Stop farm manually buttons" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="High speed server option" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Refresh farm lists" />
						</Grid>
					</Grid>
				</Grid>
				<Grid item xs={8}>
					<Grid container spacing={2}>
						<Grid item xs={12}>
							<ContentBox name="Farm list select" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Target number" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Raid style" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Option enable auto farm" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Interval setting" />
						</Grid>
					</Grid>
				</Grid>
			</Grid>
		</ContentBox>
	);
};

export default NewVillages;
